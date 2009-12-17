using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace EveMarketTool
{
    public class SearchPage : Page
    {
        enum ProfitType
        {
            ProfitPerWarpFromStartingSystem,
            MaxProfitPerWarp,
            MaxProfit
        }

        string header;
        string footer;
        NameValueCollection input = new NameValueCollection();
        TradeFinder finder;
        TradeFinderFactory factory;
        float isk = 0.0f;
        float cargo = 0.0f;
        int accounting = 0;
        string systemName;

        public SearchPage(TradeFinderFactory factory)
        {
            header = ReadFromResource("Web_Server.SearchHeader.html");
            footer = ReadFromResource("Web_Server.SearchFooter.html");
            this.factory = factory;
        }

        public override string Name()
        {
            return "Search";
        }

        public override string Render(string systemName, string charName, string charId, NameValueCollection headers, NameValueCollection query)
        {
            this.finder = factory.Create();
            this.systemName = systemName;
            input = new NameValueCollection(query);
            if (input["isk"] == null)
            {
                input["isk"] = Application.UserAppDataRegistry.GetValue("LastKnownIsk") as string;
                if (input["isk"] == null)
                {
                    input["isk"] = "";
                }
            }

            if (input["cargo"] == null)
            {
                input["cargo"] = Application.UserAppDataRegistry.GetValue("LastKnownCargoSpace") as string;
                if (input["cargo"] == null)
                {
                    input["cargo"] = "";
                }
            }

            // We should set the accounting selection to be based on the past values  - need to look up how to do this.
            //input["accounting"] = (int)Application.UserAppDataRegistry.GetValue("LastKnownAccounting");

            return "<html><title>NavBot</title><body>" + ReplaceVariables(header, input) + Conversation() + "</body></html>";
        }

        string Conversation()
        {
            if (finder == null)
                return "<p>Ok, let's get some market data! Just open the Market and change the range filter to 'Region'. Now pick an item, click on the 'Details' tab and click on the 'Export to file' button. Do this with as many items as you like! I recommend things from the 'Trade Goods' section, like Carbon, Antibodies and Reports. Come back here when you're done!</p>";
            else if (MissingElements() != "")
            {
                return "<p>Nearly there captain - I just need you to fill in your available " + MissingElements() + ", otherwise any routes I find will be meaningless.</p>";
            }
            else if (NonNumberElements() != "")
                return "<p>Sorry captain, but you'll have to tell me how much " + NonNumberElements() + " you have using numbers!</p>";
            else if (NegativeElements() != "")
                return "<p>Really captain, I don't think you'll be doing much trading with negative " + NegativeElements() + "!</p>";
            else
            {
                isk = float.Parse(input["isk"]);
                cargo = float.Parse(input["cargo"]);
                Application.UserAppDataRegistry.SetValue("LastKnownIsk", isk.ToString());
                Application.UserAppDataRegistry.SetValue("LastKnownCargoSpace", cargo.ToString());
                Application.UserAppDataRegistry.SetValue("LastKnownAccounting", accounting);
                return "<p>" + Routes() + "</p>" + footer;
            }
        }

        string MissingElements()
        {
            string result = "";
            if (input["isk"].Length == 0)
                result += "isk";
            if (input["cargo"].Length == 0)
            {
                if (result != "") result += " and ";
                result += "cargo space";
            }
            return result;
        }

        string NonNumberElements()
        {
            string result = "";
            double dummy;
            if (!double.TryParse(input["isk"], out dummy))
                result += "isk";
            if (!double.TryParse(input["cargo"], out dummy))
            {
                if (result != "") result += " and ";
                result += "cargo space";
            }
            return result;
        }

        string NegativeElements()
        {
            string result = "";
            double isk = double.Parse(input["isk"]);
            double cargo = double.Parse(input["cargo"]);
            if (isk<0)
                result += "isk";
            if (cargo<0)
            {
                if (result != "") result += " and ";
                result += "cargo space";
            }
            return result;
        }

        public string Routes()
        {
            string output = "";

            if (finder == null)
                return "";

            finder.Parameters = new Parameters(isk, cargo, systemName, TripType.SingleTrip, accounting);

            if (systemName != null)
            {
                try
                {
                    output += "<p><font size='4' face='Verdana'>Quick Cash Short-Term Profits</font></p>";
                    //output += "<table>";
                    output += ShowBestTrips(ProfitType.ProfitPerWarpFromStartingSystem);
                    //output += "</table>";
                    //output += "<br></br>";
                }
                catch (CannotFindStartingSystem)
                {
                    output += "Your current system, " + systemName + ", cannot be found in my database. Check with Tejar, he may be waiting for a database update. ";
                    output += "Sadly I can't show you the best routes from your current location, but I can still help you out with information about routes regardless of your starting point. ";
                }
            }

            output += "<p><font size='4' face='Verdana'>Best Trade Routes</font></p>";
            //output += "<table>";
            output += ShowBestTrips(ProfitType.MaxProfitPerWarp);
            //output += "</table>";
            //output += "<br></br>";

            output += "<p><font size='4' face='Verdana'>Long Travel Routes</font></p>";
            //output += "<table>";
            output += ShowBestTrips(ProfitType.MaxProfit);
            //output += "</table>";
            //output += "<br></br>";
            return output;
        }

        string ShowBestTrips(ProfitType type)
        {
            List<SingleTrip> secureTrips = null;
            List<SingleTrip> shortestTrips = null;
            string output = string.Empty;

            switch (type)
            {
                case ProfitType.ProfitPerWarpFromStartingSystem:
                    finder.SortByProfitPerWarpFromStartingSystem(true);
                    secureTrips = finder.BestHighSecTrips(2);

                    finder.SortByProfitPerWarpFromStartingSystem(false);
                    shortestTrips = finder.BestTrips(2);
                    break;

                case ProfitType.MaxProfitPerWarp:
                    finder.SortByProfitPerWarp(true);
                    secureTrips = finder.BestHighSecTrips(2);

                    finder.SortByProfitPerWarp(false);
                    shortestTrips = finder.BestTrips(2);
                    break;

                case ProfitType.MaxProfit:
                    finder.SortByProfit();
                    secureTrips = finder.BestHighSecTrips(2);
                    shortestTrips = finder.BestTrips(2);
                    break;
            }

            foreach (SingleTrip trip in secureTrips)
            {
                output += "<tr><td>" + Info(trip) + "</td></tr>";
            }

            foreach (SingleTrip trip in shortestTrips)
            {
                output += "<tr><td>" + Info(trip) + "</td></tr>";
            }

            return output;
        }

        public string Info(SingleTrip route)
        {
            return Info(route, SecurityStatus.Level.HighSec);
        }

        public string Info(SingleTrip route, SecurityStatus.Level startingTrip)
        {
            SecurityStatus.Level security = SecurityStatus.Min(startingTrip, route.Security);
            string output = "";
            
            // The output method for this had to be changed quite a bit, not sure if this is the
            // proper way to do it, since I have never used C# before -- Terracarbon.

            string TotalProfit = FormatIsk(route.Profit);
            string ProfitMargin = FormatPercent(route.ProfitMargin);
            string UnknownA = FormatPercent(route.Profit / isk);
            string FromSystem = Info(route.Source, systemName);
            string ToSystem = Info(route.Destination, route.Source.System);
            bool HighSecurity = false;

            string ProfitAWarp = "";


            if (security == SecurityStatus.Level.LowSecOnly)
            {
                HighSecurity = false;
            }
            else
            {
                HighSecurity = true;
            }

            ProfitAWarp = FormatIsk(route.ProfitPerWarp(HighSecurity));

            // test---
            output += "<table border='0' width='100%'><tr><td width='20%'><font size='2' face='Verdana'><strong>Profit: ";
            output += "</strong></font><font color='#00FF00' size='2' face='Verdana'>" + TotalProfit + " ("+ ProfitMargin + ")</font></td>";
            output += "<td width='30%'><font color='#FFFFFF' size='2' face='Verdana'><strong>From: </strong>" + Info(route.Source, systemName) + "</font>";
            output += "<font color='#00FF00' size='2' face='Verdana'> </font></td>";
            output += "<td width='30%'><font color='#FFFFFF' size='2' face='Verdana'><strong>To: </strong>" + ToSystem + "</font>";
            output += "<font color='#00FF00' size='2' face='Verdana'> </font></td>";
            output += "<td width='20%'><font color='#FFFFFF' size='2' face='Verdana'><strong>ISK/Warp: </strong></font>";
            output += "<font color='#00FF00' size='2' face='Verdana'>" + ProfitAWarp + "</font></td>";
            output += "</tr></table>";

            // endtest---



            //output += FormatIsk(route.Profit) + " profit: " + FormatPercent(route.ProfitMargin) + " margin ";
            if (isk > 0)
            {
                //output += "(" + FormatPercent(route.Profit / isk) + ") ";
            }
            //output += " from " + Info(route.Source, systemName) + " to " + Info(route.Destination, route.Source.System);

            if (security == SecurityStatus.Level.LowSecShortcut)
            {
                //output += ", " + FormatIsk(route.ProfitPerWarp(true)) + "/warp highsec, ";
                //output += ", " + FormatIsk(route.ProfitPerWarp(false)) + "/warp lowsec, ";
            }
            else if (security == SecurityStatus.Level.HighSec)
            {
                //output += ", " + FormatIsk(route.ProfitPerWarp(true)) + "/warp, ";
            }
            else
            {
                //output += ", " + FormatIsk(route.ProfitPerWarp(false)) + "/warp, ";
            }

            //string TotalCapital = string.Format("{0:N1} ISK", route.Cost);
            //string TotalCargo = string.Format("{1}m3", route.Volume);

            string TradeDetail = string.Format("You'll need <strong>{1}m3</strong> cargo space and <strong>{0:N1} ISK</strong> of capital for this trade.", route.Cost, route.Volume);

            //output += string.Format("<br>Total Cost: {0:N1} isk;  Total Cargo: {1} m3", route.Cost, route.Volume);

            //output += "<br>Purchases:<br>";
            //output += route.ListPurchases();
            //output += "Sales:<br>";
            //output += route.ListSales();

            // We use a new table here to display information a little clearer,
            // such as how much cargohold, capital and if we have to travel through
            // low security, etc.

            output += "<table border='1' cellpadding='10' cellspacing='0' width='100%' bordercolor='#C0C0C0'>";
            output += "<tr><td width='100%'><font size='2' face='Verdana'><strong>Purchases:<br>";
            output += route.ListPurchases();
            //output += "</strong></font><font color='#00FF00' size='2' face='Verdana'>Item Name</font>";
            //output += "<font color='#FFFFFF' size='2' face='Verdana'>(1632 Units) Cost: </font>";
            //output += "<font color='#FF0000' size='2' face='Verdana'>57,693,500.00 ISK<br>";
            output += "</font><font color='#FFFFFF' size='2' face='Verdana'><strong>Sales:<br>";
            output += route.ListSales();
            //output += "<a href='showinfo://2502//60004618'>test</a></strong></font><font color='#00FF00' size='2' face='Verdana'>Item Name";
            //output += "</font><font color='#FFFFFF' size='2' face='Verdana'>(1632 Units) Cost: </font>";
            //output += "<font color='#00FF00' size='2' face='Verdana'>57,693,500.00 ISK<br><br>";
            output += "<br></font><font color='#8080FF' size='2' face='Verdana'>" + TradeDetail + "</font><br>";

            if (HighSecurity == true)
            {
                output += "</font><font color='#008000' size='2' face='Verdana'>This trade travels through high security space and is considered relatively safe.<br>";
            }
            else
            {
                output += "</strong></font><font color='#FF0000' size='2' face='Verdana'><strong>This trade travels through a low security space, use caution!</strong></font>";
            }
            
            output += "</td></tr></table>";




            return output;
        }

        public string FormatIsk(float num)
        {
            string asNum;
            if(num>1000000)
                asNum = string.Format("{0:#,#,,.#}", num) + "M";
            else if(num>1000)
                asNum = string.Format("{0:#,#,.#}", num) + "K";
            else
                asNum = string.Format("{0:0.0}", num);

            if (asNum.Length == 0)
                asNum = "0.0";

            return asNum + " isk";
        }

        public string FormatPercent(float num)
        {
            string result = string.Format("{0:##%}", num);
            if (result == "%")
                result = "0%";
            return result;
        }

        public string Info(Station station)
        {
            // Terracarbon: Changed to reflect the new browser :^)
            string result = "<button type='button' onclick='CCPEVE.showInfo(" + station.TypeId + "," + station.Id + ")'>" + station.System.Name + "</button><a href='javascript:CCPEVE.setDestination(" + station.System.Id + ")'>Set</a>";
            //string result = "<a href=\"showinfo:" + station.TypeId + "//" + station.Id + "\">" + station.System.Name + "</a>";
            return result;
        }

        public string Info(Station station, string here)
        {
            string result = string.Empty;
            if (finder != null && station != null)
            {
                SolarSystem system = finder.map.GetSystem(here);
                if (system != null)
                {
                    result = Info(station, system, "");
                }
            }
            return result;
        }

        public string Pluralize(string singular, string plural, int input)
        {
            if (input == 1)
                return singular;
            else
                return plural;
        }

        public string Info(Station station, SolarSystem from)
        {
            return Info(station, from, string.Empty);
        }

        public string Info(Station station, SolarSystem from, string postfix)
        {
            string result = Info(station);
            if (finder != null && station != null && from != null)
            {
                int jumpsSecure = finder.map.DistanceBetween(from, station.System, true);
                int jumpsShortest = finder.map.DistanceBetween(from, station.System, false);
                SecurityStatus.Level security = finder.map.RouteSecurity(from, station.System);

                if (security == SecurityStatus.Level.LowSecShortcut)
                {
                    //"<FONT COLOR="#cc6600">sample text</FONT>"
                    result += " <FONT COLOR=\"#00ff33\">(" + jumpsSecure + " " + Pluralize("jump", "jumps", jumpsSecure) + ")</FONT>";
                    result += "/<FONT COLOR=\"#ff0033\">(" + jumpsShortest + " " + Pluralize("jump", "jumps", jumpsShortest) + ")</FONT>" + postfix;
                }
                else if (security == SecurityStatus.Level.HighSec)
                {
                    result += " <FONT COLOR=\"#00ff33\">(" + jumpsSecure + " " + Pluralize("jump", "jumps", jumpsSecure) + ")</FONT>" + postfix; ;
                }
                else
                {
                    result += " <FONT COLOR=\"#ff0033\">(" + jumpsShortest + " " + Pluralize("jump", "jumps", jumpsShortest) + ")</FONT>" + postfix;
                }
            }
            return result;
        }
    }
}
