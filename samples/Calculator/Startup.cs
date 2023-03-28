using Calculator.Contract;
using Calculator.Reflection;
using Calculator.WebService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using XRoadLib.Extensions.AspNetCore;

namespace Calculator
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddXRoadLib();

            services.AddSingleton<ICalculate, CalculateWebService>();
            services.AddSingleton<ISumOfIntegers, SumOfIntegersWebService>();
            services.AddSingleton<ISome, SomeWebService>();
            services.AddSingleton<CalculatorServiceManager>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseXRoadLib(routes =>
            {
                routes.MapWsdl<CalculatorServiceManager>("");
                routes.MapWebService<CalculatorServiceManager>("");
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
