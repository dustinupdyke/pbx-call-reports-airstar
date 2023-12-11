using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FileHelpers;

namespace pbx_call_reports.Models
{
    public class Snapshots
    {
        public IList<Snapshot> InboundYearly { get; set; }
        public IList<Snapshot> InboundMonthly { get; set; }
        public IList<Snapshot> InboundDaily { get; set; }
        
        public IList<Snapshot> OutboundYearly { get; set; }
        public IList<Snapshot> OutboundMonthly { get; set; }
        public IList<Snapshot> OutboundDaily { get; set; }
    }

    [DelimitedRecord(",")]
    public class Snapshot
    {
        [FieldQuoted(QuoteMode.AlwaysQuoted)] public string Name { get; set; }
        [NotMapped] public string From { get; set; }

        public int TotalCalls { get; set; }

        [FieldTrim(TrimMode.Both)]
        [FieldQuoted('"', QuoteMode.OptionalForRead, MultilineMode.AllowForRead)]
        [FieldConverter(ConverterKind.Date, "dd-MMM-yyyy hh:mm:ss")]
        public DateTime FirstCall { get; set; }

        [FieldTrim(TrimMode.Both)]
        [FieldQuoted('"', QuoteMode.OptionalForRead, MultilineMode.AllowForRead)]
        [FieldConverter(ConverterKind.Date, "dd-MMM-yyyy hh:mm:ss")]
        public DateTime MostRecentCall { get; set; }

        public double AverageTalkTime { get; set; }
    }
}