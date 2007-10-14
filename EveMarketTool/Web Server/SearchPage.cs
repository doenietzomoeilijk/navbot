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
        string header;
        string footer;
        NameValueCollection input = new NameValueCollection();
        TradeFinder finder;
        TradeFinderFactory factory;
        float isk = 0.0f;
        float cargo = 0.0f;
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

            return "<html><body>" + ReplaceVariables(header, input) + Conversation() + "</body></html>";
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

            finder.Parameters = new Parameters(isk, cargo, systemName, TripType.SingleTrip);

            if (systemName != null)
            {
                try
                {
                    finder.SortByProfitPerWarpFromStartingSystem();
                    output += "Looking for quick cash? Here's the trades that'll make us the best short-term profit:";
                    output += "<table>";
                    output += ShowBestTrips();
                    output += "</table>";
                    output += "<br></br>";
                }
                catch (CannotFindStartingSystem)
                {
                    output += "Your current system, " + systemName + ", cannot be found in my database. Check with Tejar, he may be waiting for a database update. ";
                    output += "Sadly I can't show you the best routes from your current location, but I can still help you out with information about routes regardless of your starting point. ";
                }
            }

            output += "Fancy a change of scenery? Here's the best trade routes anywhere in the galaxy:";
            output += "<table>";
            finder.SortByProfitPerWarp();
            output += ShowBestTrips();
            output += "</table>";
            output += "<br></br>";

            output += "Feel like an epic journey? Here's how much profit we could make in one long trip:";
            output += "<table>";
            finder.SortByProfit();
            output += ShowBestTrips();
            output += "</table>";
            output += "<br></br>";
            return output;
        }

        string ShowBestTrips()
        {
            string output = "";
            SingleTrip trip = finder.BestHighSecTrip();
            if (trip != null)
                output += "<tr><td>" + Info(trip) + "</td></tr>";
            trip = finder.BestLowSecTrip();
            if (trip != null)
                output += "<tr><td>" + Info(trip) + "</td></tr>";
            return output;
        }

        public string Info(SingleTrip route)
        {
            string output = "";
            if (route.Security > 0.5)
                output += "High sec: ";
            else
                output += "Low sec: ";
            
            output += FormatIsk(route.Profit) + " profit ";
            if(isk>0)
                output += "(" + FormatPercent(route.Profit / isk) + ") "; 
            output += "taking " + route.Quantity + " " + Info(route.Type) + " from " + Info(route.Source, systemName) + " to " + Info(route.Destination, route.Source.System);
            output += ", " + FormatIsk(route.ProfitPerWarp) + "/warp, ";
            output += "limited by " + route.LimitedBy + ", " + route.Security + " sec.";
            return output;
        }

        public string Info(ItemType item)
        {
            return "<a href=\"showinfo:" + item.Id + "\">" + item.Name + "</a>";
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
            string result = "<a href=\"showinfo:" + station.TypeId + "//" + station.Id + "\">" + station.System.Name + "</a>";
            return result;
        }

        public string Info(Station station, string here)
        {
            string result = Info(station);
            if (finder != null && station != null)
            {
                SolarSystem system = finder.map.GetSystem(here);
                if (system != null)
                {
                    int jumps = finder.map.DistanceBetween(system, station.System);
                    result += " (" + jumps + " " + Pluralize("jump", "jumps", jumps) + " from here)";
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
            string result = Info(station);
            if (finder != null && station != null && from != null)
            {
                int jumps = finder.map.DistanceBetween(from, station.System);
                result += " (" + jumps + " jumps)";
            }
            return result;
        }
    }
}
