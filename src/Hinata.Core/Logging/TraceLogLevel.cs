using System;

namespace Hinata.Logging
{
    public sealed class TraceLogLevel : IEquatable<TraceLogLevel>
    {
        public int Ordinal { get; private set; }

        public static readonly TraceLogLevel Trace;
        public static readonly TraceLogLevel Info;
        public static readonly TraceLogLevel Error;
        public static readonly TraceLogLevel Off;

        static TraceLogLevel()
        {
            Trace = new TraceLogLevel(0);
            Info = new TraceLogLevel(2);
            Error = new TraceLogLevel(4);
            Off = new TraceLogLevel(6);
        }

        private TraceLogLevel(int ordinal)
        {
            Ordinal = ordinal;
        }

        public override bool Equals(object obj)
        {
            var target = obj as TraceLogLevel;
            return Equals(target);
        }

        public bool Equals(TraceLogLevel other)
        {
            if (other == null) return false;
            return Ordinal == other.Ordinal;
        }

        public override int GetHashCode()
        {
            return Ordinal;
        }

        public static bool operator ==(TraceLogLevel level1, TraceLogLevel level2)
        {
            if (ReferenceEquals(level1, null))
            {
                return ReferenceEquals(level2, null);
            }

            if (ReferenceEquals(level2, null))
            {
                return false;
            }

            return level1.Ordinal == level2.Ordinal;
        }

        public static bool operator !=(TraceLogLevel level1, TraceLogLevel level2)
        {
            return !(level1 == level2);
        }

        public static TraceLogLevel FromOrdinal(int ordinal)
        {
            if (ordinal == 0) return Trace;
            if (ordinal <= 2) return Info;
            if (ordinal <= 4) return Error;
            return Off;
        }

        public static TraceLogLevel FromString(string s)
        {
            if (s.Equals("Trace", StringComparison.OrdinalIgnoreCase)) return Trace;
            if (s.Equals("Info", StringComparison.OrdinalIgnoreCase)) return Info;
            if (s.Equals("Error", StringComparison.OrdinalIgnoreCase)) return Error;

            int o;
            if (int.TryParse(s, out o))
            {
                return FromOrdinal(o);
            }

            return Off;
        }
    }
}
