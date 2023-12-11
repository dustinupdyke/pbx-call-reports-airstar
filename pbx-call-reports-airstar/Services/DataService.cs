using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using pbx_call_reports.Models;
using RestSharp;
using RestSharp.Extensions;

namespace pbx_call_reports.Services
{
    public static class DataService
    {
        /// <summary>
        /// Gets CDR2 
        /// </summary>
        public static List<NormalizedCallRecord> GetCallDetailRecords2(ApplicationConfiguration Configuration, TokenResponse token, DateTime startDate, DateTime endDate)
        {
            var client = new RestClient(Configuration.Api.Url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", $"Bearer {token.access_token}");
            request.AddParameter("application/x-www-form-urlencoded",
                $"object=cdr2&action=read&domain={token.domain}&" +
                $"start_date={startDate.ToString("yyyy-MM-dd HH:mm:ss").UrlEncode()}&" +
                $"end_date={endDate.ToString("yyyy-MM-dd HH:mm:ss").UrlEncode()}&limit=100000000", ParameterType.RequestBody);
            var dataResponse = client.Execute(request);
 
            var cdrs = JsonConvert.DeserializeObject<List<CallDetail2>>(dataResponse.Content);

            //File.WriteAllText($"test/cdr_{startDate.ToString("yyyymmmmdd")}_{endDate.ToString("yyyymmmmdd")}.json", dataResponse.Content);
                
            return cdrs.Select(cdr => new NormalizedCallRecord(cdr)).ToList();
        }
    }
}