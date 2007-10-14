using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;

namespace EveMarketTool
{
    public abstract class Page
    {
        public abstract string Name();
        public abstract string Render(string systemName, string charName, string charId, NameValueCollection headers, NameValueCollection query);

        public static string ReadFromResource(string filename)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            System.IO.StreamReader s = new System.IO.StreamReader(a.GetManifestResourceStream("EveMarketTool." + filename));
            string text = s.ReadToEnd();
            return text;
        }

        protected string ReplaceVariables(string input, string systemName, string charName, string charId)
        {
            string output = input;
            output = output.Replace("$system", systemName);
            output = output.Replace("$char_name", charName);
            output = output.Replace("$char_id", charId);
            output = output.Replace("$revision", Constants.Revision);
            return output;
        }

        protected string ReplaceVariables(string input, NameValueCollection pairs)
        {
            string output = input;
            foreach(string key in pairs.AllKeys)
                output = output.Replace("$"+key, pairs[key]);
            return output;
        }
    }
}
