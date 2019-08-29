using System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace RedditListener
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = ConfigureLogger();

            Console.ReadLine();
        }

        private static void RegisterServices()
        {
            var collection = new ServiceCollection();

            collection.AddSingleton<ILogger>(Log.Logger);
        }

        private static ILogger ConfigureLogger()
        {
            var logger = new LoggerConfiguration()
                .WriteTo
                .Console();

            Log.Logger = logger.CreateLogger();

            return Log.Logger;
        }
    }
}
