using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyNetQ;
using ImageApi.Configuration;
using ImageApi.Extensions;
using ImageIndexer.Infrastructure.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;

namespace ImageApi
{
    public class Startup
    {
        private AppSettings _appSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _appSettings = new AppSettings();
            Configuration.Bind("Settings", _appSettings);

            ConfigureLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var bus = BusFactory.Instance
                .Build(_appSettings.RabbitSettings.ConnectionString, "image-indexer", Log.Logger).Advanced;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<ILogger>(Log.Logger);
            services.AddSingleton<AppSettings>(_appSettings);
            services.AddSingleton<IAdvancedBus>(bus);
            services.AddSingleton<MessageDispatcher>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.UseAdvancedBus();

        }

        private ILogger ConfigureLogger()
        {
            var logger = new LoggerConfiguration()
                .WriteTo
                .Console();

            Log.Logger = logger.CreateLogger();

            return Log.Logger;
        }
    }
}
