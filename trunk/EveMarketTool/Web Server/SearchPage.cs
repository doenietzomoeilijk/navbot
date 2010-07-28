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

        float sales_total = 0;
        
        public SearchPage(TradeFinderFactory factory)
        {
            header = ReadFromResource("Web_Server.SearchHeader.html");
            header = ReplaceVariables(header, null, null, null);
            footer = ReadFromResource("Web_Server.SearchFooter.html");
            footer = ReplaceVariables(footer, null, null, null);
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
                if (input["isk"] == null) input["isk"] = "";
            }

            if (input["cargo"] == null)
            {
                input["cargo"] = Application.UserAppDataRegistry.GetValue("LastKnownCargoSpace") as string;
                if (input["cargo"] == null) input["cargo"] = "";
            }

            if (input["numroutes"] == null)
            {
                input["numroutes"] = Application.UserAppDataRegistry.GetValue("LastKnownNumRoutes") as string;
                if (input["numroutes"] == null) input["numroutes"] = "2";
            }
            
            if (input["accounting"] == null)
            {
                input["accounting"] = Application.UserAppDataRegistry.GetValue("LastKnownAccounting") as string;
                if (input["accounting"] == null) input["accounting"] = "0";
            }

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
                
                Application.UserAppDataRegistry.SetValue("LastKnownIsk", String.Format("{0:0.##}", isk));
                Application.UserAppDataRegistry.SetValue("LastKnownCargoSpace", cargo.ToString());
                Application.UserAppDataRegistry.SetValue("LastKnownAccounting", input["accounting"]);
                Application.UserAppDataRegistry.SetValue("LastKnownNumRoutes", input["numroutes"]);
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

            string MaxProfitStart = "";

            if (systemName != null)
            {
                try
                {
                    MaxProfitStart = ShowBestTrips(ProfitType.ProfitPerWarpFromStartingSystem);
                }
                catch (CannotFindStartingSystem)
                {
                    output += "Your current system, " + systemName + ", cannot be found in my database. Check with Tejar, he may be waiting for a database update. ";
                    output += "Sadly I can't show you the best routes from your current location, but I can still help you out with information about routes regardless of your starting point. ";
                }
            }

            string MaxProfitWarp = ShowBestTrips(ProfitType.MaxProfitPerWarp);
            string MaxProfit = ShowBestTrips(ProfitType.MaxProfit);

            string txtOut = "<p><font size='$fontsz_header' face='$fontname'>{0}</font></p>";

            output += String.Format(txtOut, "Profit Per Warp From Starting Station") + MaxProfitStart;
            output += String.Format(txtOut, "Max Profit Per Warp") + MaxProfitWarp;
            output += String.Format(txtOut, "Max Profit Overall") + MaxProfit;

            if (output.Length <= 512)
                output = "<font size='$fontsz_normal' face='$fontname'>NavBot can't find any trades available with this search criteria.</font>";

            return ReplaceVariables(output, null, null, null);
        }

        string ShowBestTrips(ProfitType type)
        {
            List<SingleTrip> secureTrips = null;
            List<SingleTrip> shortestTrips = null;
            string output = string.Empty;

            // Testing this at the moment..
            int numroutes = Convert.ToInt32(input["numroutes"]); 

            switch (type)
            {
                case ProfitType.ProfitPerWarpFromStartingSystem:
                    finder.SortByProfitPerWarpFromStartingSystem(true);
                    secureTrips = finder.BestHighSecTrips(numroutes); 

                    finder.SortByProfitPerWarpFromStartingSystem(false);
                    shortestTrips = finder.BestTrips(numroutes); 
                    break;

                case ProfitType.MaxProfitPerWarp:
                    finder.SortByProfitPerWarp(true);
                    secureTrips = finder.BestHighSecTrips(numroutes);

                    finder.SortByProfitPerWarp(false);
                    shortestTrips = finder.BestTrips(numroutes);
                    break;

                case ProfitType.MaxProfit:
                    finder.SortByProfit();
                    secureTrips = finder.BestHighSecTrips(numroutes);
                    shortestTrips = finder.BestTrips(numroutes);
                    break;
            }

            foreach (SingleTrip trip in secureTrips) output += "<tr><td>" + Info(trip) + "</td></tr>";
            foreach (SingleTrip trip in shortestTrips) output += "<tr><td>" + Info(trip) + "</td></tr>";

            return output;
        }

        public string Info(SingleTrip route)
        {
            return Info(route, SecurityStatus.Level.HighSec);
        }

        public string Info(SingleTrip route, SecurityStatus.Level startingTrip)
        {
            SecurityStatus.Level security = SecurityStatus.Min(startingTrip, route.Security);

            string output;
            string TradeSec = string.Empty;

            // todo: this differently
            int CurPercent = Convert.ToInt32(FormatPercent(route.ProfitMargin).Replace("%", ""));
            int GreyPercent = 100 - CurPercent;
            // ^--------------------^

            float taxrate = 0.01f - (Convert.ToInt32(input["accounting"]) * 0.001f);

            switch (security)
            {
                case SecurityStatus.Level.LowSecOnly:
                    TradeSec = "<b><font color='#FF0000' size='$fontsz_small' face='$fontname'>Routes through lowsec only!</font></b>";
                    break;
                case SecurityStatus.Level.LowSecShortcut:
                    TradeSec = "<font color='#FF0000' size='$fontsz_small' face='$fontname'>Route has lowsec shortcut.</font>";
                    break;
                case SecurityStatus.Level.HighSec:
                    TradeSec = "<font color='#00FF00' size='$fontsz_small' face='$fontname'>Routes through highsec only.</font>";
                    break;
                case SecurityStatus.Level.Isolated:
                    TradeSec = "</b><font color='#00FF00' size='$fontsz_small' face='$fontname'>Routes within system!</font></b>";
                    break;
            }

            // Bug here :/
            if (route.Profit - Variables.Total_Sales * taxrate <= 0) return "";
            /*{
                if (Constants.IsBetaVersion)
                {
                    return "Strange, a trade that would result in a loss was passed here. Don't report this.";
                }
                else
                {
             */
             //       return "";
             //   }
            //}

            output = ReadFromResource("Web_Server.TradePage.html");

            // Replace $Identifiers ..
            output = output.Replace("$TO", Info(route.Destination, route.Source.System));
            output = output.Replace("$FROM", Info(route.Source, systemName));
            output = output.Replace("$HISK", FormatIsk(route.ProfitPerWarp(true)));
            output = output.Replace("$LISK", FormatIsk(route.ProfitPerWarp(false)));
            output = output.Replace("$CURPERCENT", String.Format("{0}", CurPercent));
            output = output.Replace("$ENDPERCENT", String.Format("{0}", GreyPercent));
            output = output.Replace("$TRADESEC", TradeSec);
            output = output.Replace("$LISTP", route.ListPurchases());
            output = output.Replace("$LISTS", route.ListSales());
            output = output.Replace("$CNEED", String.Format("{0:0.##}m3", route.Volume));
            output = output.Replace("$RNEED", String.Format("{0:0.##} ISK", route.Cost));
            output = output.Replace("$TAXTOTAL", String.Format("{0:0.##} ({1:0.##}%)", Variables.Total_Sales * taxrate, taxrate * 100));
            output = output.Replace("$PROFIT", FormatIsk(route.Profit - (Variables.Total_Sales * taxrate)) + " (" + FormatPercent(route.ProfitMargin) + ")");

            output = ReplaceVariables(output, null, null, null);

            return output;
        }

        public string FormatIsk(float num)
        {
            string asNum;
            if(num>1000000)
                asNum = string.Format("{0:#,#,,.##}", num) + "M";
            else if(num>1000)
                asNum = string.Format("{0:#,#,.#}", num) + "K";
            else
                asNum = string.Format("{0:0.###}", num);

            if (asNum.Length == 0)
                asNum = "0.0";

            return asNum + " ISK";
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
            // Changed in latest build (showinfo:// to CCPEVE.showInfo) -- Terry
            string result = "<button type='button' onclick='CCPEVE.showInfo(" + station.TypeId + "," + station.Id + ")'>" + station.System.Name + "</button> <a href='javascript:CCPEVE.setDestination(" + station.System.Id + ")'>Set</a>";
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
