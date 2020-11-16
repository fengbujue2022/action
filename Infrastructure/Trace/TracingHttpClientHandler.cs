using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Trace
{
    public class TracingHttpClientHandler : DelegatingHandler
    {
        private readonly ILogger<TracingHttpClientHandler> logger;
        private readonly ITraceIdAccessor traceIdAccessor;

        public TracingHttpClientHandler(ILogger<TracingHttpClientHandler> logger, ITraceIdAccessor traceIdAccessor)
        {
            this.logger = logger;
            this.traceIdAccessor = traceIdAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            logger.LogInformation(traceIdAccessor.TraceId);//TODO
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
