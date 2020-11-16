using APIClient;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Infrastructure.Autofac.Modules;
using Infrastructure.Trace;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = BuildServicePorvider(args);
            var taskTracing = provider.GetService<TaskTracing>();
            var bootstrap = provider.GetService<Bootstrap>();
            await taskTracing.Run(() => bootstrap.Main());
        }

        public static IServiceProvider BuildServicePorvider(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(args, services);
            var builder = new ContainerBuilder();
            builder.RegisterModule<AutoResolveClassModule>();
            builder.RegisterModule<PropertyAutowiredModule>();
            builder.Populate(services);
            return new AutofacServiceProvider(builder.Build());
        }

        public static void ConfigureServices(string[] args, IServiceCollection services)
        {
            //configuration
            var configuation = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile($"appsettings.json", true, true)
                .AddCommandLine(args, new Dictionary<string, string>() {
                    {"-emailKey","EmailKey" },
                })
                .Build();

            services.AddOptions()
                .Configure<Config>(configuation);
            //serilog
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            services.AddLogging((builder) =>
            {
                builder.AddSerilog(dispose: true, logger: logger);
            });
            //task trace
            services.AddTaskTracing();

            //refit
            services.AddRefitClient<HitokotoApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://v1.hitokoto.cn"))
                .AddHttpMessageHandler((provider) =>
                {
                    return provider.GetService<TracingHttpClientHandler>();
                });
        }
    }
}
