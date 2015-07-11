namespace Hinata.Logging
{
    public class AccessLog
    {
        public string ServerName { get; set; }

        public string UserName { get; set; }

        public string Url { get; set; }

        public string HttpMethod { get; set; }

        public string Path { get; set; }

        public string Query { get; set; }

        public string Form { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string UserHostAddress { get; set; }

        public string UserAgent { get; set; }
    }
}