using EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ImageApi.Extensions
{
    public static class ApplicationBuilderExtentions
    {
        private static IConfiguration _configuration { get; set; }
        private static IAdvancedBus _messenger { get; set; }

        public static IApplicationBuilder UseAdvancedBus(this IApplicationBuilder app)
        {
            _messenger = app.ApplicationServices.GetService<IAdvancedBus>();
            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();

            //lifetime.ApplicationStarted.Register(OnStarted);
            //press Ctrl+C to reproduce if your app runs in Kestrel as a console app
            lifetime.ApplicationStopping.Register(OnStopping);

            return app;
        }

        private static void OnStopping()
        {
            _messenger.Dispose();
        }
    }
}