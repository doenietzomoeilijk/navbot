using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using NUnit.Framework;
using EveMarketTool.Tests.Mock_Objects;

namespace EveMarketTool
{
    public class ReportsPage : Page
    {
        string header;
        string reportsFoundFooter;
        string noReportsFoundFooter;
        TradeFinderFactory factory;

        public ReportsPage(TradeFinderFactory factory)
        {
            this.factory = factory;
            header = ReadFromResource("Web_Server.ReportsHeader.html");
            reportsFoundFooter = ReadFromResource("Web_Server.ReportsFoundFooter.html");
            noReportsFoundFooter = ReadFromResource("Web_Server.ReportsNotFoundFooter.html");
        }

        public override string Name()
        {
            return "Reports";
        }

        public override string Render(string systemName, string charName, string charId, NameValueCollection headers, NameValueCollection query)
        {
            if (query["archive"] == "old")
            {
                factory.ArchiveOldLogs();
            }
            else if (query["archive"] == "all")
            {
                factory.ArchiveAllLogs();
            }

            return "<html><body>" + ReplaceVariables(header, systemName, charName, charId) + ShowReportList() + "</body></html>";
        }

        string ShowReportList()
        {
            factory.ArchiveOutOfDateLogs();
            string[] filenames = factory.ReportList();

            if (filenames.Length == 0)
            {
                return "<p>Actually, captain, we don't have any market reports at the moment. To download some, use the \"Export Data\" button in the market.</p>" + noReportsFoundFooter;
            }
            else
            {
                string output = "<p>I have the following reports in my databanks, captain. To download more, use the \"Export Data\" button in the market!</p>";
                output += "<table>";
                List<ReportInfo> reportList = new List<ReportInfo>(filenames.Length);

                foreach (string f in filenames)
                {
                    reportList.Add(new ReportInfo(f));
                }

                reportList.Sort(ReportInfo.CompareByDate);

                foreach (ReportInfo ri in reportList)
                {
                    TimeSpan age = DateTime.Now - ri.Date;
                    output += "<tr><td>" + ri.ItemName + "</td><td>" + ri.Region + "</td><td>" + (int)age.TotalHours + " hours and " + age.Minutes + " minutes old" + "</td></tr>";
                }

                output += "</table>";
                output += reportsFoundFooter;
                return output;
            }
        }
    }
}
