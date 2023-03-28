using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Calculator
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            /*Console.WriteLine("GenericMethodBuilder started...");
            GenericMethodBuilder.Execute();
            Console.WriteLine(); Console.WriteLine();
            Console.WriteLine("GenericTypeBuilder started...");
            GenericTypeBuilder.Execute();
            Console.ReadLine();*/
            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
