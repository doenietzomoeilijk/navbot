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
            reportsFoundFooter = ReplaceVariables(reportsFoundFooter, null, null, null);
            noReportsFoundFooter = ReplaceVariables(noReportsFoundFooter, null, null, null);

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

            string theReportList = ShowReportList();

            return "<html><title>NavBot</title><body>" + ReplaceVariables(header, systemName, charName, charId) + ShowReportList() + "</body></html>";
        }

        string ShowReportList()
        {
            factory.ArchiveOutOfDateLogs();
            string[] filenames = factory.ReportList();

            if (filenames.Length == 0)
            {
                return ReplaceVariables("<font size='$fontsz_normal' face='$fontname'>We don't seem to have any data available! To download data, use the <strong>Export Data</strong> button in the market.</font>" + noReportsFoundFooter, null, null, null);
            }
            else
            {
                string output = "";
                output += "<font color='$fontcolor' size='$fontsz_normal' face='$fontname'>";
                output += "I have the following data available to me. To download more data, use the <strong>Export Data</strong> button in the market.</font><br><br>";

                List<ReportInfo> reportList = new List<ReportInfo>(filenames.Length);

                output += "<table border='0' cellpadding='5' width='100%'>";
                output += "<tr><td width='40%' bgcolor='$itemsbg_color'><font color='$fontcolor' size='$fontsz_small' face='$fontname'><strong>Item Name</strong></font></td>";
                output += "<td width='20%' bgcolor='$itemsbg_color'><font color='$fontcolor' size='$fontsz_small' face='$fontname'><strong>Region</strong></font></td>";
                output += "<td width='40%' bgcolor='$itemsbg_color'><font color='$fontcolor' size='$fontsz_small' face='$fontname'><strong>Age</strong></font></td></tr>";


                foreach (string f in filenames)
                {
                    reportList.Add(new ReportInfo(f));
                }

                reportList.Sort(ReportInfo.CompareByDate);

                foreach (ReportInfo ri in reportList)
                {
                    TimeSpan age = DateTime.Now - ri.Date;
                    output += "<tr><td width='40%' bgcolor='$mainbg_color'><font color='$fontcolor' size='$fontsz_small' face='$fontname'><strong>" + ri.ItemName + "</strong></font></td>";
                    output += "<td width='20%' bgcolor='$mainbg_color'><font color='$fontcolor' size='$fontsz_small' face='$fontname'>" + ri.Region + "</font></td>";
                    output += "<td width='40%' bgcolor='$mainbg_color'><font color='$fontcolor' size='$fontsz_small' face='$fontname'>" + (int)age.TotalHours + " hours and " + age.Minutes + " minutes ago</font></td></tr>";
                }

                output = ReplaceVariables(output, null, null, null); // App.config settings replace
                output += "</table>";
                output += reportsFoundFooter;
                
                return output;
            }
        }
    }
}
