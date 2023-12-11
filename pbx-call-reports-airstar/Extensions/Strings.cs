using System.Globalization;

namespace pbx_call_reports.Extensions
{
    public static class Strings
    {
        public static string ToTitleCase(this string o)
        {
            var textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(o.ToLower()); 
        }
    }
}