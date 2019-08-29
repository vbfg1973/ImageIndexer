using System;
using Serilog;

namespace RedditListener
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = ConfigureLogger();

            logger.Information(Environment.GetEnvironmentVariable("REDDIT_USER"));
            logger.Information(Environment.GetEnvironmentVariable("REDDIT_REDIRECT"));

            Console.ReadLine();
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
