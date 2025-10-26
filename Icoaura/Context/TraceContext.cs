
namespace Icoaura.Context
{
    public static class TraceContext
    {
        [ThreadStatic]
        private static string? _traceId;

        public static string TraceId
        {
            get
            {
                if (_traceId == null)
                    _traceId = Guid.NewGuid().ToString();
                return _traceId;
            }
            set => _traceId = value;
        }

        public static void Reset() => _traceId = null;
    }
}
