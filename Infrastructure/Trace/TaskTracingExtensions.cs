using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Trace
{
    public static class TaskTracingExtensions
    {
        public static IServiceCollection AddTaskTracing(this IServiceCollection services)
        {
            services.AddSingleton<ITraceIdAccessor, TraceIdAccessor>();
            services.AddSingleton<TaskTracing, TaskTracing>();
            services.AddSingleton<TracingHttpClientHandler, TracingHttpClientHandler>();
            return services;
        }
    }
}
