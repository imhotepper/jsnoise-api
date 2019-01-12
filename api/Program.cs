using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using BeatPulse;

namespace CoreJsNoise
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
        .UseBeatPulse(options=>
        {
            options.ConfigurePath(path:"/api/health") //default hc
                .ConfigureTimeout(milliseconds:1500) // default -1 infinitely
                .ConfigureDetailedOutput(detailedOutput:true); //default false
        })
                .UseStartup<Startup>();
    }
}