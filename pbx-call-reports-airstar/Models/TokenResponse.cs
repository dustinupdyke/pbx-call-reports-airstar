namespace pbx_call_reports.Models
{
    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public bool legacy { get; set; }
        public string domain { get; set; }
        public string apiversion { get; set; }
    }
}