using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BeatPulse;
using BeatPulse.UI;
using CoreJsNoise.Domain;
using CoreJsNoise.Dto;
using CoreJsNoise.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Npgsql;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;
using SpaApiMiddleware;
using BeatPulse.System;
using BeatPulse.Network;
using CoreJsNoise.GraphQL;
using CoreJsNoise.Jobs;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.OpenApi.Models;

namespace CoreJsNoise
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            
            
            var conStr = Configuration.GetConnectionString("DefaultConnection");
            var pgConn = Environment.GetEnvironmentVariable("DATABASE_URL");

            if (!string.IsNullOrWhiteSpace(pgConn)){
                conStr = HerokuPGParser.ConnectionHelper.BuildExpectedConnectionString(pgConn);
                
                  var uri = new Uri(pgConn);            
                var username = uri.UserInfo.Split(':')[0];
                var password = uri.UserInfo.Split(':')[1];
                conStr = 
                "host=" + uri.Host +
                "; Database=" + uri.AbsolutePath.Substring(1) +
                "; Username=" + username +
                "; Password=" + password + 
                "; Port=" + uri.Port +
                "; SSL Mode=Require; Trust Server Certificate=true;";
                
            }
           
            services.AddDbContext<PodcastsCtx>(options => options.UseNpgsql(conStr));
                                    
            // services.AddBeatPulse(setup =>
            // {
            //     setup.AddNpgSql(conStr);
            //     setup.AddWorkingSetLiveness(536870912);
              
            // });
          //  services.AddBeatPulseUI();

            services.AddScoped<PodcastsCtx>();
            services.AddScoped<FeedUpdaterService>();
            services.AddScoped<RssReader>();
            
            services.AddAutoMapper();
            services.AddMediatR();
            
            services.AddCors();
          
            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
            services.AddScoped<JsNoiseSchema>();
            services.AddGraphQL(o =>
            {
                o.ExposeExceptions = true;
                
            }).AddGraphTypes(ServiceLifetime.Scoped)
                .AddUserContextBuilder(httpContext => httpContext.User)
               // .AddDataLoader()
               ;
            
            services.AddMvc(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHostedService<FeedUpdaterJob>();
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "jsnoise api", Version = "v1" });
            });
  
               services .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
                .AddBasicAuthentication(
                    options =>
                    {
                        var userName  =Environment.GetEnvironmentVariable("USER_NAME");
                        if (string.IsNullOrWhiteSpace(userName)) userName = "user";
                        var pass  =Environment.GetEnvironmentVariable("PASSWORD");
                        if (string.IsNullOrWhiteSpace(pass)) pass = "pass";
                        
                        options.Realm = "My Application";
                        options.Events = new BasicAuthenticationEvents
                        {
                            OnValidatePrincipal = context =>
                            {
                                if ((context.UserName == userName) && (context.Password == pass))
                                {
                                    var claims = new List<Claim>
                                    {
                                        new Claim(ClaimTypes.Name, context.UserName, context.Options.ClaimsIssuer)
                                    };

                                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, BasicAuthenticationDefaults.AuthenticationScheme));
                                    context.Principal = principal;
                                }
                                else 
                                {
                                    // optional with following default.
                                    // context.AuthenticationFailMessage = "Authentication failed."; 
                                }

                                return Task.CompletedTask;
                            }
                        };
                    });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseCors(cfg => cfg.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());


            app.UseGraphQL<JsNoiseSchema>();
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions());
            
            app.UseSpaApiOnly();
            
            
        // TODO: uncomment later   app.UseBeatPulseUI();

        
            
           // app.UseResponseCompression();
        
            //basic auth
            app.UseAuthentication();

          //  app.UseDefaultFiles();
           // app.UseStaticFiles();
            app.UseCors(cfg => cfg.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "public api");
                c.RoutePrefix = string.Empty;

            });

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                if (serviceScope.ServiceProvider.GetService<PodcastsCtx>() != null)
                {
                    var ctx = serviceScope.ServiceProvider.GetService<PodcastsCtx>();
                    new DatabaseFacade(ctx).Migrate();
                }
            }
        }
    }
}