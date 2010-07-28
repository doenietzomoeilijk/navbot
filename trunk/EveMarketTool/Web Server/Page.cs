using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Reflection;
using System.Configuration;

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
            output = output.Replace("$credits", Constants.Credits);

            output = output.Replace("$cfgurl", ConfigurationSettings.AppSettings["URLPrefix"]);
            // Fonts
            output = output.Replace("$fontname", ConfigurationSettings.AppSettings["FontName"]);
            output = output.Replace("$fontsz_header", ConfigurationSettings.AppSettings["FontSize_Header"]);
            output = output.Replace("$fontsz_normal", ConfigurationSettings.AppSettings["FontSize_Normal"]);
            output = output.Replace("$fontsz_small", ConfigurationSettings.AppSettings["FontSize_Small"]);
            output = output.Replace("$fontsz_tiny", ConfigurationSettings.AppSettings["FontSize_Tiny"]);
            // Colors
            output = output.Replace("$fontcolor", ConfigurationSettings.AppSettings["DefaultFont_Color"]);
            output = output.Replace("$bg_color", ConfigurationSettings.AppSettings["PageBG_Color"]);
            output = output.Replace("$mainbg_color", ConfigurationSettings.AppSettings["MainBG_Color"]);
            output = output.Replace("$itemsbg_color", ConfigurationSettings.AppSettings["ItemsBG_Color"]);
            output = output.Replace("$tcolor", ConfigurationSettings.AppSettings["TextColor"]);
            output = output.Replace("$lcolor", ConfigurationSettings.AppSettings["LinkColor"]);
            output = output.Replace("$vcolor", ConfigurationSettings.AppSettings["VLinkColor"]);
            output = output.Replace("$acolor", ConfigurationSettings.AppSettings["ALinkColor"]);
            output = output.Replace("$bgimage", ConfigurationSettings.AppSettings["Background_Image"]);

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
