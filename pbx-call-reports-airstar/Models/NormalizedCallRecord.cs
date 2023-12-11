using System;
using pbx_call_reports.Extensions;

namespace pbx_call_reports.Models
{
    public class NormalizedCallRecord
    {
        public string ToUsername { get; set; }
        public string FromUsername { get; set; }
        public string To { get; set; }
        public string ToUri { get; set; }
        public string From { get; set; }
        public string FromUri { get; set; }
        public DateTime Start { get; set; }
        public int TalkTime { get; set; }
        public bool IsInbound {get;set;}
        public string TerminatesTo { get; set; }

        public NormalizedCallRecord() { }
        
        public NormalizedCallRecord(CallDetail2 detail)
        {
            this.FromUsername = detail.Orig_from_name;
            this.ToUsername = detail.Orig_to_user;
            
            this.From = detail.Orig_sub;
            this.To = detail.Term_sub;
            
            this.FromUri = detail.Orig_from_uri;
            this.ToUri = detail.Orig_req_uri;
            
            this.Start = detail.Time_start.FromUnixTime().ToLocalTime();
            this.TalkTime = Convert.ToInt32(detail.Time_talking);
            this.TerminatesTo = detail.Term_to_uri;
            this.IsInbound = !this.FromUri.Contains(Program.Configuration.Api.ClientSip, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}