using System.Security.Claims;
using CoreJsNoise.Domain;
using CoreJsNoise.GraphQL;
using CoreJsNoise.Services;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SpaApiMiddleware;
using ZNetCS.AspNetCore.Authentication.Basic;
using ZNetCS.AspNetCore.Authentication.Basic.Events;

using MediatR;
using AutoMapper;
using CoreJsNoise.Config;
using CoreJsNoise.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();


      var conStr = builder.Configuration.GetConnectionString("DefaultConnection");
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
           
builder.Services.AddDbContext<PodcastsCtx>(options => options.UseNpgsql(conStr));
                                    
            // services.AddBeatPulse(setup =>
            // {
            //     setup.AddNpgSql(conStr);
            //     setup.AddWorkingSetLiveness(536870912);
              
            // });
          //  services.AddBeatPulseUI();

builder.Services.AddScoped<PodcastsCtx>();
builder.Services.AddScoped<FeedUpdaterService>();
builder.Services.AddScoped<RssReader>();
            
builder.Services.AddMediatR(typeof(ProducerGetAllRequest).Assembly);
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


builder.Services.AddCors();
          
builder.Services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService));
builder.Services.AddScoped<JsNoiseSchema>();
builder.Services.AddGraphQL(o =>
            {
                o.ExposeExceptions = true;
                
            }).AddGraphTypes(ServiceLifetime.Scoped)
                .AddUserContextBuilder(httpContext => httpContext.User)
               // .AddDataLoader()
               ;
            
// builder.Services.AddMvc(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining 1 or more Swagger documents
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "jsnoise api", Version = "v1" });
            });
  
builder.Services .AddAuthentication(BasicAuthenticationDefaults.AuthenticationScheme)
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
            

var app = builder.Build();


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


//
// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
//
// app.UseHttpsRedirection();
//
// app.UseAuthorization();

app.MapControllers();


// using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
// {
//     if (serviceScope.ServiceProvider.GetService<PodcastsCtx>() != null)
//     {
//         var ctx = serviceScope.ServiceProvider.GetService<PodcastsCtx>();
//         new DatabaseFacade(ctx).Migrate();
//     }
// }

app.Run();