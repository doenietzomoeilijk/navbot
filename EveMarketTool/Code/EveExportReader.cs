using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace EveMarketTool
{
    public class EveExportReader
    {
        public static string dataDir = "../../../EveMarketTool/data/";

        public EveExportReader()
        {
        }

        public void ReadFromDataDir(string filename)
        {
            ReadFromFullPath(dataDir + filename);
        }

        void ReadFromString(string text)
        {
            string[] lines = text.Split(new string[] { "\"\r\n\"" }, StringSplitOptions.RemoveEmptyEntries);
            string[] header = ReadWords(lines[0]);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int line = 1; line < lines.Length; line++)
            {
                dict.Clear();
                string[] words = ReadWords(lines[line]);
                for (int field = 0; field < words.Length; field++)
                {
                    dict[header[field]] = words[field];
                }
                InterpretRow(dict);
            }
        }

        public void ReadFromFullPath(string filename)
        {
            string text = System.IO.File.ReadAllText(filename);
            ReadFromString(text);
        }

        public void ReadFromResource(string filename)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            System.IO.StreamReader s = new System.IO.StreamReader(a.GetManifestResourceStream("EveMarketTool." + filename));
            string text = s.ReadToEnd();
            ReadFromString(text);
        }

        /// Convert Eve ID numbers from input files (they use . as a thousand seperator)
        public int ParseId(string input) 
        {
            string converted = input.Replace(".", "");
            if (converted.Length > 0)
            {
                return int.Parse(converted);
            }
            else
            {
                return -1; // invalid id
            }
        }

        /// Convert Eve numbers from input files (they use . as a thousand seperator and , as a decimal seperator)
        private static readonly CultureInfo GermanCulture = new CultureInfo("de-DE");
        public float ParseNumber(string input)
        {
            return float.Parse(input, GermanCulture);
        }

        private string[] ReadWords(string line)
        {
            string[] words = line.Split(new string[] { "\";\"" }, StringSplitOptions.None);
            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].TrimStart(new char[] { '"' });
                words[i] = words[i].TrimEnd(new char[] { '"' });
            }
            return words;
        }

        protected virtual void InterpretRow(Dictionary<string, string> fields)
        {
            foreach(string s in fields.Keys)
            {
                Console.Write(s + "=" + fields[s] + " ");
            }
            Console.WriteLine();
        }
    }
}
