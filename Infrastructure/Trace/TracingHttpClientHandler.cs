using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

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
            try
            {
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                //TODO
                //logger.LogInformation(traceIdAccessor.TraceId);
                logger.LogInformation(await new RawHttpContent(request, response, logger).AsString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }

    public class RawHttpContent
    {
        private readonly HttpRequestMessage request;
        private readonly HttpResponseMessage response;
        private readonly ILogger logger;

        public IEnumerable<string> _allowedContentTypes = new string[] { "xml", "urlencoded", "text", "json", "html" };

        public RawHttpContent(HttpRequestMessage request, HttpResponseMessage response, ILogger logger)
        {
            this.request = request;
            this.response = response;
            this.logger = logger;
        }

        /// <summary>
        ///  TODO 需要考虑一些特殊情况 待验证
        /// </summary>
        /// <returns></returns>
        public async Task<string> AsString()
        {
            try
            {
                var builder = new StringBuilder();
                //request
                builder.AppendLine($"{request.Method} {request.Headers.Host}/{request.RequestUri} HTTP/{request.Version}");
                foreach (var header in request.Headers)
                {
                    builder.AppendLine($"{header.Key}: {header.Value}");
                }
                builder.AppendLine();
                if (request.Content != null)
                {
                    var requestBody = await request.Content.ReadAsStringAsync();
                    builder.AppendLine(requestBody);
                }

                builder.AppendLine();

                //response
                builder.AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.StatusCode}");
                foreach (var header in response.Headers)
                {
                    builder.AppendLine($"{header.Key}: { string.Join("; ", header.Value)}");
                }
                builder.AppendLine();
                if (response.Content != null)
                {
                    var mediaType = response.Content.Headers?.ContentType?.MediaType;
                    if (mediaType == null || _allowedContentTypes.Any(x => mediaType.Contains(x, StringComparison.OrdinalIgnoreCase)))
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        builder.AppendLine(responseBody);
                    }
                }
                return builder.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError("resolve RawHttpContent failed :", ex);
                return string.Empty;
            }
        }

    }
}
