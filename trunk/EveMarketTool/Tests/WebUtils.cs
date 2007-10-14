using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace EveMarketTool.Tests
{
    public class WebUtils
    {
        public static string ReadPage(string url)
        {
            return ReadPageWithHeaders(url, new NameValueCollection(), null);
        }

        public static string PlainText(string html)
        {
            string result = Regex.Replace(html, @"<(.|\n)*?>", string.Empty);
            return result;
        }

        public static string ReadPageWithHeaders(string url, NameValueCollection headers, NameValueCollection outputHeaders)
        {
            WebRequest myWebRequest = WebRequest.Create(url);
            if (headers.Count > 0)
            {
                WebHeaderCollection webHeaders;
                webHeaders = new WebHeaderCollection();
                foreach (string key in headers.Keys)
                {
                    webHeaders.Add(key, headers[key]);
                }
                myWebRequest.Headers = webHeaders;
            }
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Stream ReceiveStream = myWebResponse.GetResponseStream();
            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(ReceiveStream, encode);
            string strResponse = readStream.ReadToEnd();
            readStream.Close();
            if (outputHeaders != null)
            {
                foreach (string key in myWebResponse.Headers.Keys)
                {
                    outputHeaders.Add(key, myWebResponse.Headers[key]);
                }
            }
            myWebResponse.Close();
            return strResponse;
        }
    }
}
