using System.Xml.Serialization;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace pbx_call_reports.Models
{
    /*
    public class CallDetailRecordXml
    {
        [XmlElement("batch_dura")]
        public int Batch_dura { get; set; }

        [XmlElement("batch_tim_beg")]
        public string Batch_tim_beg { get; set; }

        [XmlElement("by_domain")]
        public string By_domain { get; set; }

        [XmlElement("by_sub")] public string By_sub { get; set; }

        [XmlElement("disposition")]
        public string Disposition { get; set; }

        [XmlElement("notes")] public string Notes { get; set; }

        [XmlElement("orig_callid")]
        public string Orig_callid { get; set; }

        [XmlElement("orig_domain")]
        public string Orig_domain { get; set; }

        [XmlElement("orig_from_name")]
        public string Orig_from_name { get; set; }

        [XmlElement("orig_from_uri")]
        public string Orig_from_uri { get; set; }

        [XmlElement("orig_sub")] public string Orig_sub { get; set; }

        [XmlElement("orig_to_user")]
        public string Orig_to_user { get; set; }

        [XmlElement("pac")] public string Pac { get; set; }
        [XmlElement("reason")] public string Reason { get; set; }

        [XmlElement("release_text")]
        public string Release_text { get; set; }

        [XmlElement("route_to")] public string Route_to { get; set; }

        [XmlElement("term_callid")]
        public string Term_callid { get; set; }

        [XmlElement("term_domain")]
        public string Term_domain { get; set; }

        [XmlElement("term_sub")] public string Term_sub { get; set; }

        [XmlElement("term_to_uri")]
        public string Term_to_uri { get; set; }

        [XmlElement("time_answer")]
        public string Time_answer { get; set; }

        [XmlElement("time_release")]
        public string Time_release { get; set; }
    }

    [XmlRoot("xml")]
    public class CallDetailRecordListXml
    {
        [XmlElement("cdr")]
        public List<CallDetailRecordXml> CallDetailRecordsXml { get; set; }
    }
    */
    
    public class CallDetail
    {
        [JsonProperty(PropertyName = "batch_dura")]
        public int Batch_dura { get; set; }

        [JsonProperty(PropertyName = "batch_tim_beg")]
        public string Batch_tim_beg { get; set; }

        [JsonProperty(PropertyName = "by_domain")]
        public string By_domain { get; set; }

        [JsonProperty(PropertyName = "by_sub")] 
        public string By_sub { get; set; }

        [JsonProperty(PropertyName = "disposition")]
        public string Disposition { get; set; }

        [JsonProperty(PropertyName = "notes")] 
        public string Notes { get; set; }

        [JsonProperty(PropertyName = "orig_callid")]
        public string Orig_callid { get; set; }

        [JsonProperty(PropertyName = "orig_domain")]
        public string Orig_domain { get; set; }

        [JsonProperty(PropertyName = "orig_from_name")]
        public string Orig_from_name { get; set; }

        [JsonProperty(PropertyName = "orig_from_uri")]
        public string Orig_from_uri { get; set; }

        [JsonProperty(PropertyName = "orig_sub")] 
        public string Orig_sub { get; set; }

        [JsonProperty(PropertyName = "orig_to_user")]
        public string Orig_to_user { get; set; }

        [JsonProperty(PropertyName = "pac")] 
        public string Pac { get; set; }
        
        [JsonProperty(PropertyName = "reason")] 
        public string Reason { get; set; }

        [JsonProperty(PropertyName = "release_text")]
        public string Release_text { get; set; }

        [JsonProperty(PropertyName = "route_to")] 
        public string Route_to { get; set; }

        [JsonProperty(PropertyName = "term_callid")]
        public string Term_callid { get; set; }

        [JsonProperty(PropertyName = "term_domain")]
        public string Term_domain { get; set; }

        [JsonProperty(PropertyName = "term_sub")] 
        public string Term_sub { get; set; }

        [JsonProperty(PropertyName = "term_to_uri")]
        public string Term_to_uri { get; set; }

        [JsonProperty(PropertyName = "time_answer")]
        public string Time_answer { get; set; }

        [JsonProperty(PropertyName = "time_release")]
        public string Time_release { get; set; }
    }
    

    public class CallDetail2
    {
        [JsonProperty(PropertyName = "domain")]
        public string Domain { get; set; }
        
        [JsonProperty(PropertyName = "territory")]
        public string Territory { get; set; }
        
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        
        [JsonProperty(PropertyName = "cdr_id")]
        public string Cdr_id { get; set; }
        
        [JsonProperty(PropertyName = "orig_sub")]
        public string Orig_sub { get; set; }
        
        [JsonProperty(PropertyName = "orig_from_uri")]
        public string Orig_from_uri { get; set; }
        
        [JsonProperty(PropertyName = "orig_from_name")]
        public string Orig_from_name { get; set; }
        
        [JsonProperty(PropertyName = "orig_to_user")]
        public string Orig_to_user { get; set; }
        
        [JsonProperty(PropertyName = "orig_req_uri")]
        public string Orig_req_uri { get; set; }
        
        [JsonProperty(PropertyName = "orig_req_user")]
        public string Orig_req_user { get; set; }
        
        [JsonProperty(PropertyName = "by_sub")]
        public string By_sub { get; set; }
        
        [JsonProperty(PropertyName = "term_sub")]
        public string Term_sub { get; set; }
        
        [JsonProperty(PropertyName = "term_to_uri")]
        public string Term_to_uri { get; set; }
        
        [JsonProperty(PropertyName = "time_start")]
        public string Time_start { get; set; }
        
        [JsonProperty(PropertyName = "time_answer")]
        public string Time_answer { get; set; }
        
        [JsonProperty(PropertyName = "time_release")]
        public string Time_release { get; set; }
        
        [JsonProperty(PropertyName = "duration")]
        public string Duration { get; set; }
        
        [JsonProperty(PropertyName = "time_talking")]
        public string Time_talking { get; set; }
        
        [JsonProperty(PropertyName = "hide")]
        public string Hide { get; set; }
        
        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }
    }
}