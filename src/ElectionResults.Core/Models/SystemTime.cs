using System;

namespace ElectionResults.Core.Models
{
    public class SystemTime
    {
        private static DateTimeOffset _now;

        public static DateTimeOffset Now
        {
            get
            {
                if (_now == DateTimeOffset.MinValue)
                {
                    return DateTimeOffset.UtcNow;
                }
                return _now;
            }
            set => _now = value;
        }

        public static void Reset()
        {
            _now = DateTimeOffset.MinValue;
        }
    }
}
