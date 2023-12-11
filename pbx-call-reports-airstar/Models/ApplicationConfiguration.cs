namespace pbx_call_reports.Models
{
    public class ApplicationConfiguration
    {
        public ApiDetails Api { get; set; }
        public ApplicationDetails Application { get; set; }

        public class ApiDetails
        {
            public string Url { get; set; }
            public string ClientId { get; set; }
            public string ClientSecret { get; set; }
            public string ClientSip { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class ApplicationDetails
        {
            public bool IsDebug { get; set; }
            public bool IsInterofficeFiltering { get; set; }
            public bool IsInterofficeFilterLogging { get; set; }
            public CallerDetails[] ReportOnCallers { get; set; }
            public string OutputDirectory { get; set; }
            public string OutputFileName { get; set; }

            public class CallerDetails
            {
                public string Name { get; set; }
                public string[] Extensions { get; set; }
            }
        }
    }
}