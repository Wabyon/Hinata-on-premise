using System;
using Newtonsoft.Json;

namespace Hinata.Logging
{
    public sealed class TraceLogMessage
    {
        public DateTime DateTime { get; private set; }
        public object Command { get; private set; }
        public string Key { get; private set; }

        public long Duration { get; private set; }

        public TraceLogMessage(object command, string key) : this(command, key, 0)
        {
        }

        public TraceLogMessage(object command, string key, long duration)
        {
            DateTime = DateTime.UtcNow;
            Command = command;
            Key = key;
            Duration = duration;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}