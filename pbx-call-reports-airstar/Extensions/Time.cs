using System;

namespace pbx_call_reports.Extensions
{
    public static class Time
    {
        public static string ConvertToMinutesAndSeconds(this double seconds)
        {
            var t = TimeSpan.FromSeconds( seconds );
            return $"{t.Minutes:D2}:{t.Seconds:D2}"; 
        }
        
        public static DateTime FromUnixTime(this string unixTime)
        {
            var l = Convert.ToDouble(unixTime);
            return l.FromUnixTime();
        }
        
        public static DateTime FromUnixTime(this double unixTime)
        {
            var startDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var time = startDate.AddSeconds(unixTime);
            return time;
        }
    }
}