using System;
using System.Threading;
using System.Threading.Tasks;


namespace Infrastructure.Trace
{
    public class TaskTracing
    {
        private readonly ITraceIdAccessor _traceIdAccessor;
        public TaskTracing(ITraceIdAccessor traceIdAccessor)
        {
            _traceIdAccessor = traceIdAccessor;
        }

        public Task Run(Func<Task> function)
        {
            return Task.Run(async () =>
            {
                _traceIdAccessor.TraceId = Guid.NewGuid().ToString();
                await function();
                _traceIdAccessor.TraceId = null;
            });
        }

        public Task Run<Result>(Func<Task<Result>> function)
        {
            return Task.Run(async () =>
            {
                _traceIdAccessor.TraceId = Guid.NewGuid().ToString();
                var result = await function();
                _traceIdAccessor.TraceId = null;
                return result;
            });
        }
    }

    public interface ITraceIdAccessor
    {
        string TraceId { get; set; }
    }

    public class TraceIdAccessor : ITraceIdAccessor
    {
        private static AsyncLocal<TraceIdHolder> _traceIdCurrent = new AsyncLocal<TraceIdHolder>();

        public string TraceId
        {
            get
            {
                return _traceIdCurrent.Value?.TraceId;
            }
            set
            {
                var holder = _traceIdCurrent.Value;
                if (holder != null)
                {
                    holder.TraceId = null;
                }

                if (value != null)
                {
                    _traceIdCurrent.Value = new TraceIdHolder { TraceId = value };
                }
            }
        }

        private class TraceIdHolder
        {
            public string TraceId;
        }
    }
}
