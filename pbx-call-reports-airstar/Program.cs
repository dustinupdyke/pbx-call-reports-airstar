using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using FileHelpers;
using Newtonsoft.Json;
using NLog;
using pbx_call_reports.Extensions;
using pbx_call_reports.Models;
using pbx_call_reports.Services;
using RestSharp;
using RestSharp.Extensions;

namespace pbx_call_reports
{
    class Program
    {
        public static ApplicationConfiguration Configuration { get; set; }

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                _log.Info("PBX report generator starting...");
                var configRaw = File.ReadAllText("config/appsettings.json");
                Configuration = JsonConvert.DeserializeObject<ApplicationConfiguration>(configRaw);
                _log.Trace("Configuration loaded successfully...");

                // reports data clean up
                if (!Directory.Exists($"{Configuration.Application.OutputDirectory}data/"))
                {
                    Directory.CreateDirectory($"{Configuration.Application.OutputDirectory}data/");
                }
                var d = new DirectoryInfo($"{Configuration.Application.OutputDirectory}data/");
                var files = d.GetFiles("*.json");
                foreach(var file in files )
                {
                    try
                    {
                        if (!file.Name.StartsWith((DateTime.Now.Year).ToString()))
                            file.Delete();
                    }
                    catch {  }
                }
                
                // need to figure out start/end date
                var firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
                var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var today = DateTime.Now.Date;
                
                List<NormalizedCallRecord> yearlies = new List<NormalizedCallRecord>();
                List<NormalizedCallRecord> monthlies;
                List<NormalizedCallRecord> dailies;

                if (!Configuration.Application.IsDebug)
                {
                    var tokenResponse = TokenService.GetResponse(Configuration);
                    
                    var monthData = new List<NormalizedCallRecord>();
                    var raw = "";
                    for (var i = 1; i < DateTime.Now.Month; i++) // store every month prior to this month
                    {
                        var thisFile = $"{Configuration.Application.OutputDirectory}data/{DateTime.Now.Year}-{i}.json";
                        if (!File.Exists(thisFile))
                        {
                            var mStart = new DateTime(DateTime.Now.Year, i, 1);
                            var mEnd = mStart.AddMonths(1).AddTicks(-1);
                            monthData = DataService.GetCallDetailRecords2(Configuration, tokenResponse, mStart, mEnd);
                            raw = JsonConvert.SerializeObject(monthData);
                            File.WriteAllText(thisFile, raw);
                        }
                        else
                        {
                            raw = File.ReadAllText(thisFile);
                            monthData = JsonConvert.DeserializeObject<List<NormalizedCallRecord>>(raw);
                        }
                        yearlies.AddRange(monthData);
                    }
                    
                    // now add current month
                    var endOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddTicks(-1);
                    monthData = DataService.GetCallDetailRecords2(Configuration, tokenResponse, new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1), endOfThisMonth);
                    yearlies.AddRange(monthData);
                    
                    
                    // now slice and dice as usual
                    //yearlies = DataService.GetCallDetailRecords2(Configuration, tokenResponse, firstDayOfYear, endDate);
                    monthlies = yearlies.Where(o => o.Start >= firstDayOfMonth).ToList();
                    dailies = monthlies.Where(o => o.Start >= today).ToList();
                }
                else
                {
                    var raw = File.ReadAllText("test/cdr2.json");
                    var o2 = JsonConvert.DeserializeObject<List<CallDetail2>>(raw);
                    
                    yearlies = new List<NormalizedCallRecord>();
                    foreach(var cdr in o2)
                        yearlies.Add(new NormalizedCallRecord(cdr));
                    
                    monthlies = new List<NormalizedCallRecord>();
                    foreach(var cdr in o2)
                        monthlies.Add(new NormalizedCallRecord(cdr));
                    
                    dailies = monthlies.Where(o => o.Start >= today).ToList();
                }
                
                var snapshots = new Snapshots();
                
                // *** OUTBOUND ***
                // do yearly
                snapshots.OutboundYearly = SnapshotBuilderService.BuildOutbound(Configuration, yearlies, firstDayOfYear);
                // do monthly
                snapshots.OutboundMonthly = SnapshotBuilderService.BuildOutbound(Configuration, monthlies, firstDayOfMonth);
                // do daily
                snapshots.OutboundDaily = SnapshotBuilderService.BuildOutbound(Configuration, dailies, today);
                
                // *** INBOUND *** 
                // do yearly
                snapshots.InboundYearly = SnapshotBuilderService.BuildInbound(Configuration, yearlies, firstDayOfYear);
                // do monthly
                snapshots.InboundMonthly = SnapshotBuilderService.BuildInbound(Configuration, monthlies, firstDayOfMonth);
                // do daily
                snapshots.InboundDaily = SnapshotBuilderService.BuildInbound(Configuration, dailies, today);
                
                
                SnapshotBuilderService.WriteExcelFile(snapshots, Configuration);

                var msg = "PBX report generator ran successfully.";
                _log.Info(msg);
                Console.WriteLine(msg);
            }
            catch (Exception e)
            {
                _log.Error(e);
                Environment.Exit(1);
            }
        }
    }
}