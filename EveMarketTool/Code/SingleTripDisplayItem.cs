using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace EveMarketTool
{
    class SingleTripDisplayItem : ListViewItem
    {
        private int profitPerWarp;
        private int profit;

        public SingleTripDisplayItem(Map map, SingleTrip data, Parameters parameters)
        {
            Text = data.TypeName;
            profit = (int)data.Profit;
            int profitPercentage = (profit * 100) / (int)parameters.Isk;

            SolarSystem startingSystem = map.GetSystem(parameters.StartingSystem);
            if (startingSystem == null)
                profitPerWarp = (int)data.ProfitPerWarp;
            else
            {
                data.StartingSystem = startingSystem;
                profitPerWarp = (int)data.ProfitPerWarpFromStartingSystem;
            }

            int jumpsFromStart = map.DistanceBetween(startingSystem, data.Source.System);
            int jumps = data.Jumps;
            string source = data.Source.Name;
            string destination = data.Destination.Name;
            float security = data.Security;
            LimitingFactor limitedBy = data.LimitedBy;

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
            list.Columns.Add("Item", 150, HorizontalAlignment.Left);
            list.Columns.Add("ISK/warp", 70, HorizontalAlignment.Right);
            list.Columns.Add("%Profit", 90, HorizontalAlignment.Right);
            list.Columns.Add("Security", 70, HorizontalAlignment.Center);
            if (showJumpToSource)
                list.Columns.Add("Jmp to Src", 80, HorizontalAlignment.Center);
            else
                list.Columns.Add("-", 0, HorizontalAlignment.Center);
            list.Columns.Add("Jumps in Route", 110, HorizontalAlignment.Center);
            list.Columns.Add("Source", 300, HorizontalAlignment.Left);
            list.Columns.Add("Destination", 300, HorizontalAlignment.Left);
            list.Columns.Add("Limited by", 100, HorizontalAlignment.Center);
        }

        public static int CompareByProfitPerWarp(SingleTripDisplayItem a, SingleTripDisplayItem b)
        {
            return b.profitPerWarp.CompareTo(a.profitPerWarp); // highest profit comes first
        }

        public static int CompareByProfit(SingleTripDisplayItem a, SingleTripDisplayItem b)
        {
            return b.profit.CompareTo(a.profit); // highest profit comes first
        }
    }
}
