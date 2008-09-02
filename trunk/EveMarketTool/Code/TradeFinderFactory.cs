using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EveMarketTool
{
    public class TradeFinderFactory
    {
        string logFilePath;
        static Map map;
        static ItemDatabase itemDatabase;

        public TradeFinderFactory(string logFilePath)
        {
            this.logFilePath = logFilePath;
        }

        public virtual TradeFinder Create()
        {
            return Create(logFilePath);
        }

        protected static TradeFinder Create(string logFilePath)
        {
            if (map == null)
            {
                map = new Map();
            }
            itemDatabase = new ItemDatabase();
            Market market = new Market(itemDatabase, map);
            return Create(map, market, logFilePath, null);
        }

        public static TradeFinder Create(Map map, Market market, string logFilePath, Parameters parameters)
        {
            ArchiveOutOfDateLogs(logFilePath);

            string[] logs = ReportList(logFilePath);

            if (logs.Length == 0)
                return null;

            map.ClearMarketData();

            foreach (string s in logs)
            {
                market.ReadFromFullPath(s);
            }

            return new TradeFinder(map, market, parameters);
        }

        public virtual string[] ReportList()
        {
            return ReportList(logFilePath);
        }

        public static string[] ReportList(string logFilePath)
        {
            return Directory.GetFiles(logFilePath + "\\", "*.txt", System.IO.SearchOption.TopDirectoryOnly);
        }

        public static void ArchiveOutOfDateLogs(string logFilePath)
        {
            string[] files = Directory.GetFiles(logFilePath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            List<string> sortedFiles = new List<string>(files);
            sortedFiles.Sort();
            files = sortedFiles.ToArray();
            List<FileInfo> moveList = new List<FileInfo>();
            for (int i = 0; i < files.Length-1; i++)
            {
                string current = files[i];
                string next = files[i + 1];
                if (RegionAndItem(current) == RegionAndItem(next))
                {
                    string oldest = OldestOf(current, next);
                    moveList.Add(new FileInfo(oldest));
                }
            }

            ArchiveFiles(logFilePath, moveList);
        }

        static void ArchiveFiles(string logFilePath, List<FileInfo> files)
        {
            if (!Directory.Exists(logFilePath + "\\Archive"))
            {
                Directory.CreateDirectory(logFilePath + "\\Archive");
            }

            foreach (FileInfo file in files)
            {
                string destName = file.DirectoryName + "\\Archive\\" + file.Name;
                if (File.Exists(destName))
                {
                    File.Delete(destName);
                }
                file.MoveTo(file.DirectoryName + "\\Archive\\" + file.Name);
            }
        }

        public virtual void ArchiveOutOfDateLogs()
        {
            ArchiveOutOfDateLogs(this.logFilePath);
        }

        public virtual void ArchiveOldLogs()
        {
            string[] files = Directory.GetFiles(logFilePath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            List<FileInfo> moveList = new List<FileInfo>();

            for (int i = 0; i < files.Length; i++)
            {
                ReportInfo current = new ReportInfo(files[i]);
                if(DateTime.Now - current.Date > new TimeSpan(24, 0, 0))
                    moveList.Add(new FileInfo(files[i]));
            }

            ArchiveFiles(logFilePath, moveList);
        }

        public virtual void ArchiveAllLogs()
        {
            string[] files = Directory.GetFiles(logFilePath, "*.txt", System.IO.SearchOption.TopDirectoryOnly);
            List<FileInfo> moveList = new List<FileInfo>();

            for (int i = 0; i < files.Length; i++)
            {
                moveList.Add(new FileInfo(files[i]));
            }

            ArchiveFiles(logFilePath, moveList);
        }

        static string RegionAndItem(string fullpath)
        {
            ReportInfo info = new ReportInfo(fullpath);
            return info.Region + " - " + info.ItemName;
        }

        static string OldestOf(string one, string two)
        {
            ReportInfo a = new ReportInfo(one);
            ReportInfo b = new ReportInfo(two);
            if (a.Date < b.Date)
                return one;
            else
                return two;
        }

    }
}
