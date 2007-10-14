using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EveMarketTool
{
    class RoundTripDisplayItem : ListViewItem
    {
        private int profitPerWarp;
        private int profit;

        public RoundTripDisplayItem(Map map, RoundTrip data, Parameters parameters)
        {
            Text = data.There.TypeName + "/" + data.BackAgain.TypeName;
            profit = (int)data.Profit;
            int profitPercentage = (profit * 100) / (int)parameters.Isk;
            
            profitPerWarp = (int)data.ProfitPerWarp;
            SolarSystem startingSystem = map.GetSystem(parameters.StartingSystem);
            int jumpsFromStart = map.DistanceBetween(startingSystem, data.There.Source.System);
            int jumps = data.There.Jumps;
            string source = data.There.Source.Name;
            string destination = data.There.Destination.Name;
            float security = data.Security;
            string limitedBy = data.There.LimitedBy.ToString();
            if(data.BackAgain.ItemType != null)
                limitedBy += " / " + data.BackAgain.LimitedBy;

            SubItems.Add(profitPerWarp.ToString());
            SubItems.Add(profitPercentage + "%");
            SubItems.Add(security.ToString());
            SubItems.Add(jumpsFromStart.ToString());
            SubItems.Add(jumps.ToString());
            SubItems.Add(source);
            SubItems.Add(destination);
            SubItems.Add(limitedBy.ToString());
        }

        public static void SetupColumns(ListView list, bool showJumpToSource)
        {
            list.Columns.Clear();
            list.Columns.Add("Items", 150, HorizontalAlignment.Left);
            list.Columns.Add("ISK/warp", 70, HorizontalAlignment.Right);
            list.Columns.Add("%Profit", 90, HorizontalAlignment.Right);
            list.Columns.Add("Security", 70, HorizontalAlignment.Center);
//            if (showJumpToSource)
  //              list.Columns.Add("Jmp to Src", 80, HorizontalAlignment.Center);
    //        else
                list.Columns.Add("-", 0, HorizontalAlignment.Center);
            list.Columns.Add("Jumps in Route", 110, HorizontalAlignment.Center);
            list.Columns.Add("Source", 300, HorizontalAlignment.Left);
            list.Columns.Add("Destination", 300, HorizontalAlignment.Left);
            list.Columns.Add("Limited by", 150, HorizontalAlignment.Center);
        }

        public static int CompareByProfitPerWarp(RoundTripDisplayItem a, RoundTripDisplayItem b)
        {
            return b.profitPerWarp.CompareTo(a.profitPerWarp); // highest profit comes first
        }

        public static int CompareByProfit(RoundTripDisplayItem a, RoundTripDisplayItem b)
        {
            return b.profit.CompareTo(a.profit); // highest profit comes first
        }
    }
}
