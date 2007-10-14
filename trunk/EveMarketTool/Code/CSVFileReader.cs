using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class CSVFileReader
    {
        public CSVFileReader()
        {
        }

        public void ReadFromFullPath(string filename)
        {
            string[] lines = System.IO.File.ReadAllLines(filename);
            Dictionary<string, string> dict = new Dictionary<string,string>();
            string[] header = lines[0].Split(new char[] { ',' });
            for(int i=1; i<lines.Length; i++)
            {
                dict.Clear();
                string[] words = lines[i].Split(new char[] {','});
                for(int field=0;field<words.Length;field++)
                {
                    dict[header[field]] = words[field];
                }
                InterpretRow(dict);
            }
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
