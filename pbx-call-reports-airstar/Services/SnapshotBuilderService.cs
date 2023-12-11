using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FileHelpers;
using NLog;
using pbx_call_reports.Extensions;
using pbx_call_reports.Models;

namespace pbx_call_reports.Services
{
    public static class SnapshotBuilderService
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static List<Snapshot> BuildInbound(ApplicationConfiguration Configuration, IEnumerable<NormalizedCallRecord> cdrs, DateTime startDate)
        {
            var snapshots = new List<Snapshot>();

            var inboundCallDestinations = cdrs.Where(o=>o.IsInbound && o.TerminatesTo.Contains(Configuration.Api.ClientSip, StringComparison.CurrentCultureIgnoreCase)).Select(o => o.To).Distinct();
            foreach (var destination in inboundCallDestinations)
            {
                try
                {
                    var allCalls = cdrs.Where(o => string.Equals(o.To, destination, StringComparison.CurrentCultureIgnoreCase));

                    var calls = allCalls;
                    if (Configuration.Application.IsInterofficeFiltering)
                    {
                        // inter-office calls as three digit to and from values
                        calls = allCalls.Where(o => !(o.To is { Length: 3 } && o.From is { Length: 3 }));
                        _log.Info($"There were {allCalls.Count() - calls.Count()} inter-office calls removed");
                        _log.Info($"Generating inbound snapshot of {allCalls.Count()} all calls, and {calls.Count()} after removing inter-office calls");

                        if (Configuration.Application.IsInterofficeFilterLogging)
                        {
                            var filteredCalls = allCalls.Where(o => o.To is { Length: 3 } && o.From is { Length: 3 });
                            var thisFile = $"{Configuration.Application.OutputDirectory}data/inbound-filtered-{DateTime.Now.ToString("yyyy-MM-dd-THHmmss")}.json";
                            SerializationHelper.WriteToJsonFile(thisFile, filteredCalls, true);
                        }
                    }
                    else
                    {
                        _log.Info($"Generating inbound snapshot of {allCalls.Count()} all calls. Removing inter-office calls is turned off");
                    }

                    var s = new Snapshot();
                    
                    s.Name = destination;
                    s.TotalCalls = calls.Count();
                    s.FirstCall = (calls.OrderBy(o => o.Start).First().Start);
                    s.MostRecentCall = (calls.OrderByDescending(o => o.Start).First().Start);
                    s.AverageTalkTime = calls.Average(o => Math.Abs(o.TalkTime));

                    if (!snapshots.Exists(o => string.Equals(o.Name, destination, StringComparison.InvariantCultureIgnoreCase)))
                        snapshots.Add(s);
                }
                catch (Exception e)
                {
                    _log.Error($"Exception processing call record: {e}");
                }
            }

            snapshots = snapshots.OrderByDescending(o => o.TotalCalls).ToList();

            if (Configuration.Application.IsDebug)
            {
                var engine = new FileHelperEngine<Snapshot>();
                engine.HeaderText = engine.GetFileHeader();

                var output = new DirectoryInfo(Configuration.Application.OutputDirectory);
                if (!output.Exists)
                    output.Create();

                var snapshotName = "monthly";
                if (startDate == DateTime.Now.Date)
                    snapshotName = "daily";

                var fileName = $"{snapshotName}.csv";

                engine.WriteFile($"{output}{Path.DirectorySeparatorChar}{fileName}", snapshots);
            }

            return snapshots;
        }
        
        public static List<Snapshot> BuildOutbound(ApplicationConfiguration Configuration, IEnumerable<NormalizedCallRecord> cdrs, DateTime startDate)
        {
            var snapshots = new List<Snapshot>();

            try
            {
                var originalExtensions = cdrs.Select(o => o.From).Distinct();
                foreach (var extension in originalExtensions)
                {
                    if (string.IsNullOrEmpty(extension))
                        continue;

                    var caller = Configuration.Application.ReportOnCallers.FirstOrDefault(x => x.Extensions.Contains(extension, StringComparer.InvariantCultureIgnoreCase));

                    if (caller == null || string.IsNullOrEmpty(caller.Name))
                        continue;
                    
                    Console.WriteLine($"Processing calls for {caller.Name}");
                    
                    try
                    {
                        var allCalls = cdrs.Where(x => caller.Extensions.Contains(x.From, StringComparer.InvariantCultureIgnoreCase));
                        
                        var calls = allCalls;
                        if (Configuration.Application.IsInterofficeFiltering)
                        {
                            // inter-office calls as three digit to and from values
                            calls = allCalls.Where(o => !(o.To is { Length: 3 } && o.From is { Length: 3 }));
                            _log.Info($"There were {allCalls.Count() - calls.Count()} inter-office calls removed");
                            _log.Info($"Generating outbound snapshot of {allCalls.Count()} all calls, and {calls.Count()} after removing inter-office calls");

                            if (Configuration.Application.IsInterofficeFilterLogging)
                            {
                                var filteredCalls = allCalls.Where(o => o.To is { Length: 3 } && o.From is { Length: 3 });
                                var thisFile = $"{Configuration.Application.OutputDirectory}data/outbound-filtered-{DateTime.Now.ToString("yyyy-MM-dd-THHmmss")}.json";
                                SerializationHelper.WriteToJsonFile(thisFile, filteredCalls, true);
                            }
                        }
                        else
                        {
                            _log.Info($"Generating outbound snapshot of {allCalls.Count()} all calls. Removing inter-office calls is turned off");
                        }
                        
                        var s = new Snapshot();

                        //.Replace(configuration.Api.ClientSip, "")
                        //"orig_from_uri": "sip:17248201868@ACCoy.20905.service",
                        //                                  ACCoy.20905.service
                        var from = calls.First().FromUri.Replace(Configuration.Api.ClientSip, "").Replace("@", "").Replace("sip:", "");
                        s.From = from;

                        s.Name = caller.Name;
                        s.TotalCalls = calls.Count();
                        s.FirstCall = (calls.OrderBy(o => o.Start).First().Start);
                        s.MostRecentCall = (calls.OrderByDescending(o => o.Start).First().Start);
                        s.AverageTalkTime = calls.Average(o => Math.Abs(o.TalkTime));

                        if (!snapshots.Exists(o => caller.Extensions.Contains(o.From, StringComparer.InvariantCultureIgnoreCase)))
                            snapshots.Add(s);
                    }
                    catch (Exception e)
                    {
                        _log.Error($"Exception processing call record: {e}");
                    }
                }

                snapshots = snapshots.OrderByDescending(o => o.TotalCalls).ToList();

                if (Configuration.Application.IsDebug)
                {
                    var engine = new FileHelperEngine<Snapshot>();
                    engine.HeaderText = engine.GetFileHeader();

                    var output = new DirectoryInfo(Configuration.Application.OutputDirectory);
                    if (!output.Exists)
                        output.Create();

                    var snapshotName = "monthly";
                    if (startDate == DateTime.Now.Date)
                        snapshotName = "daily";

                    var fileName = $"{snapshotName}.csv";

                    engine.WriteFile($"{output}{Path.DirectorySeparatorChar}{fileName}", snapshots);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return snapshots;
        }
        
        public static void WriteExcelFile(Snapshots snapshots, ApplicationConfiguration configuration)
        {
            var output = new DirectoryInfo(configuration.Application.OutputDirectory);
            if (!output.Exists)
                output.Create();

            using (var document = SpreadsheetDocument.Create(
                $"{output}{Path.DirectorySeparatorChar}{configuration.Application.OutputFileName}",
                SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                var sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                var sheet = new Sheet() {Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Snapshot"};

                sheets.Append(sheet);

                // daily

                var headerRow = new Row();

                var cell = new Cell();
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue($"Daily Snapshot - as of Date / Time {DateTime.Now.ToString("M/d/yyyy hh:m")}");

                headerRow.AppendChild(cell);
                sheetData.AppendChild(headerRow);

                var columns = new List<string>();
                columns.Add("Name");
                columns.Add("Total Calls Made");
                columns.Add("Most Recent Call");
                columns.Add("First Call");
                columns.Add("Average Talk Time");
                
                columns.Add("Total Calls Received");
                columns.Add("Most Recent Received");
                columns.Add("Average Received Talk Time");

                headerRow = new Row();
                foreach (var column in columns)
                {
                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column);
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (var item in snapshots.OutboundDaily)
                {
                    var newRow = new Row();

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.Name.ToTitleCase());
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.TotalCalls.ToString());
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.MostRecentCall.ToString(CultureInfo.InvariantCulture));
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.FirstCall.ToString(CultureInfo.InvariantCulture));
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.AverageTalkTime.ConvertToMinutesAndSeconds());
                    newRow.AppendChild(cell);

                    
                    var received = snapshots.InboundDaily.FirstOrDefault(o => o.Name is not null && o.Name.Equals(item.From));
                    if(received != null)
                    {
                        //Total Calls Received
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.TotalCalls.ToString());
                        newRow.AppendChild(cell);
                    
                        //Most Recent Received
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.MostRecentCall.ToString(CultureInfo.InvariantCulture));
                        newRow.AppendChild(cell);
                    
                        //Average Received Talk Time
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.AverageTalkTime.ConvertToMinutesAndSeconds());
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                sheetData.AppendChild(new Row());
                sheetData.AppendChild(new Row());
                sheetData.AppendChild(new Row());

                // monthly

                headerRow = new Row();
                cell = new Cell();
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue($"Month to Date Snapshot - as of Date / Time {DateTime.Now.ToString("M/d/yyyy h:mm")}");

                headerRow.AppendChild(cell);
                sheetData.AppendChild(headerRow);

                headerRow = new Row();

                foreach (var column in columns)
                {
                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column);
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (var item in snapshots.OutboundMonthly)
                {
                    var newRow = new Row();

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.Name.ToTitleCase());
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.TotalCalls.ToString());
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.MostRecentCall.ToString(CultureInfo.InvariantCulture));
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.FirstCall.ToString(CultureInfo.InvariantCulture));
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.AverageTalkTime.ConvertToMinutesAndSeconds());
                    newRow.AppendChild(cell);
                    
                    var received = snapshots.InboundMonthly.FirstOrDefault(o => o.Name is not null && o.Name.Equals(item.From));
                    if(received != null)
                    {
                        //Total Calls Received
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.TotalCalls.ToString());
                        newRow.AppendChild(cell);
                    
                        //Most Recent Received
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.MostRecentCall.ToString(CultureInfo.InvariantCulture));
                        newRow.AppendChild(cell);
                    
                        //Average Received Talk Time
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.AverageTalkTime.ConvertToMinutesAndSeconds());
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                sheetData.AppendChild(new Row());
                sheetData.AppendChild(new Row());
                sheetData.AppendChild(new Row());

                // yearly

                headerRow = new Row();
                cell = new Cell();
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue($"Year to Date Snapshot - as of Date / Time {DateTime.Now.ToString("M/d/yyyy h:mm")}");

                headerRow.AppendChild(cell);
                sheetData.AppendChild(headerRow);

                headerRow = new Row();

                foreach (var column in columns)
                {
                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(column);
                    headerRow.AppendChild(cell);
                }

                sheetData.AppendChild(headerRow);

                foreach (var item in snapshots.OutboundYearly)
                {
                    var newRow = new Row();

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.Name.ToTitleCase());
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.TotalCalls.ToString());
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.MostRecentCall.ToString(CultureInfo.InvariantCulture));
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.FirstCall.ToString(CultureInfo.InvariantCulture));
                    newRow.AppendChild(cell);

                    cell = new Cell();
                    cell.DataType = CellValues.String;
                    cell.CellValue = new CellValue(item.AverageTalkTime.ConvertToMinutesAndSeconds());
                    newRow.AppendChild(cell);
                    
                    var received = snapshots.InboundYearly.FirstOrDefault(o => o.Name is not null && o.Name.Equals(item.From));
                    if(received != null)
                    {
                        //Total Calls Received
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.TotalCalls.ToString());
                        newRow.AppendChild(cell);
                    
                        //Most Recent Received
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.MostRecentCall.ToString(CultureInfo.InvariantCulture));
                        newRow.AppendChild(cell);
                    
                        //Average Received Talk Time
                        cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(received.AverageTalkTime.ConvertToMinutesAndSeconds());
                        newRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(newRow);
                }

                workbookPart.Workbook.Save();
            }
        }
    }
}