using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FlurlTestService
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.Run(async context =>
            {
                var result = new Result()
                {
                    Path = context.Request.Path,
                    Query = context.Request.QueryString.Value
                };
                var json = JsonConvert.SerializeObject(result);
                context.Request.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            });
        }
    }

    public class Result
    {
        public string Path { get; set; }

        public string Query { get; set; }
    }
}
