using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EveMarketTool
{
    class ReportInfo
    {
        private string itemName;
        public string ItemName
        {
            get { return itemName; }
        }

        private string region;
        public string Region
        {
            get { return region; }
        }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
        }

        public static int CompareByDate(ReportInfo a, ReportInfo b)
        {
            return DateTime.Compare(a.date, b.date);
        }

        public ReportInfo(string filename)
        {
            FileInfo fi = new FileInfo(filename);
            string basic = fi.Name;
            string[] parts = basic.Split(new string[] { "-" }, StringSplitOptions.None);
            if (parts != null && parts.Length >= 3)
            {
                region = parts[0].Trim();

                // some items have "-" in their name. Reconstruct the item name using all parts 
                // except the last one, which is always the date
                itemName = parts[1];
                for(int i=2;i<parts.Length-1;i++)
                    itemName += "-" + parts[i];
                
                itemName = itemName.Trim();

                string dateStr = parts[parts.Length-1].Trim();
                string datePart = dateStr.Substring(0, dateStr.IndexOf(' '));
                string timePart = dateStr.Substring(dateStr.IndexOf(' ')+1);
                string[] dates = datePart.Split(new char[] { '.' });
                if (dates.Length >= 3 && timePart.Length>=6)
                {
                    int year, month, day, hour, minute, second;
                    int.TryParse(dates[0], out year);
                    int.TryParse(dates[1], out month);
                    int.TryParse(dates[2], out day);
                    int.TryParse(timePart.Substring(0, 2), out hour);
                    int.TryParse(timePart.Substring(2, 2), out minute);
                    int.TryParse(timePart.Substring(4, 2), out second);
                    date = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(year, month, day, hour, minute, second));
                    if (date > DateTime.Now)
                        date = DateTime.Now;
                }
            }
        }
    }
}
