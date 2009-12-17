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

            return "<html><title>NavBot</title><body>" + ReplaceVariables(header, systemName, charName, charId) + ShowReportList() + "</body></html>";
        }

        string ShowReportList()
        {
            factory.ArchiveOutOfDateLogs();
            string[] filenames = factory.ReportList();

            if (filenames.Length == 0)
            {
                return "<font color='C0C0C0' size='3' face='Verdana'><b>Uh oh!</b> We don't seem to have any data available! To download data, use the <strong>Export Data</strong> button in the market.</font><br><br>" + noReportsFoundFooter;
            }
            else
            {

                string output = "";

                output += "<font color='#C0C0C0' size='3' face='Verdana'>";
                output += "I have the following data available to me. To download more data, use the <strong>Export Data</strong> button in the market.</font><br><br>";

                List<ReportInfo> reportList = new List<ReportInfo>(filenames.Length);

                // Start of Table [Item Name, Region, Age]

                output += "<table border='0' cellpadding='5' width='100%'>";
                output += "<tr><td width='40%' bgcolor='#444444'><font color='#FFFFFF' size='2' face='Verdana'><strong>Item Name</strong></font></td>";
                output += "<td width='20%' bgcolor='#444444'><font color='#FFFFFF' size='2' face='Verdana'><strong>Region</strong></font></td>";
                output += "<td width='40%' bgcolor='#444444'><font color='#FFFFFF' size='2' face='Verdana'><strong>Age</strong></font></td></tr>";


                foreach (string f in filenames)
                {
                    reportList.Add(new ReportInfo(f));
                }

                reportList.Sort(ReportInfo.CompareByDate);

                //int ColumnColor = 0;

                foreach (ReportInfo ri in reportList)
                {
                    
                    // Table listed items page added by Terracarbon (For Dominion!)
                    TimeSpan age = DateTime.Now - ri.Date;
                    //output += "<tr><td>" + ri.ItemName + "</td><td>" + ri.Region + "</td><td>" + (int)age.TotalHours + " hours and " + age.Minutes + " minutes old" + "</td></tr>";
                    
                    // Column Style (A) 
                    output += "<tr><td width='40%' bgcolor='#151515'><font color='#FFFFFF' size='2' face='Verdana'><strong>" + ri.ItemName + "</strong></font></td>";
                    output += "<td width='20%' bgcolor='#151515'><font color='#FFFFFF' size='2' face='Verdana'>" + ri.Region + "</font></td>";
                    output += "<td width='40%' bgcolor='#151515'><font color='#FFFFFF' size='2' face='Verdana'>" + (int)age.TotalHours + " hours and " + age.Minutes + " minutes ago</font></td></tr>";
                    

                }

                output += "</table>";
                output += reportsFoundFooter;
                return output;
            }
        }
    }
}
