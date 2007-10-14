using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Windows.Forms;

namespace EveMarketTool
{
    public class PageServer
    {
        public delegate void ErrorMessageHandler(string text, string extra);
        public event ErrorMessageHandler ReportError;

        private Dictionary<string, Page> pages = new Dictionary<string,Page>();
        private Page defaultPage;

        private string url;
        public string Url
        {
            get { return url; }
        }

        HttpListener httpListener;

        public PageServer()
        {
            httpListener = new HttpListener();
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            url = "http://localhost:9999/";
            string prefix = string.Format("http://+:{0}/", 9999);
            httpListener.Prefixes.Add(prefix);
            httpListener.Start();
            IAsyncResult result = httpListener.BeginGetContext(new AsyncCallback(WebRequest), httpListener);
        }

        public void Close()
        {
            httpListener.Close();
            httpListener = null;
        }

        public void Reset()
        {
            pages.Clear();
            defaultPage = null;
        }

        public void AddPage(Page page)
        {
            pages.Add("/" + page.Name(), page);
            if (defaultPage == null)
                defaultPage = page;
        }

        void WebRequest(IAsyncResult ar)
        {
            if (httpListener == null) return;

            try
            {
                HttpListenerContext context = httpListener.EndGetContext(ar);
                string systemName = context.Request.Headers["Eve.solarsystemname"];
                string charName = context.Request.Headers["Eve.charname"];
                string charId = context.Request.Headers["Eve.charid"];

                string htmlOutput;
                string pageKey = context.Request.Url.AbsolutePath;
                pageKey = pageKey.TrimEnd(new char[] { '/' });
                if (pageKey == "/trustme") // special case - send request trust string in header
                {
                    context.Response.AddHeader("eve.trustme", "http://localhost:9999/");
                    htmlOutput = "<html><body>Once you have enabled trust, please click <a href=\"http://localhost:9999/Welcome\">here</a>. You may have to set trust manually using the menus above.</body></html>";
                }
                else if (pages.ContainsKey(pageKey))
                {
                    htmlOutput = pages[pageKey].Render(systemName, charName, charId, context.Request.Headers, context.Request.QueryString);
                }
                else if (defaultPage != null) // default to first page if name is not known
                {
                    htmlOutput = defaultPage.Render(systemName, charName, charId, context.Request.Headers, context.Request.QueryString);
                }
                else
                {
                    htmlOutput = "<html><body>Oops - I seem to have lost my mind. Please report this to Tejar at once!</body></html>";
                }

                byte[] htmlData = System.Text.UTF8Encoding.UTF8.GetBytes(htmlOutput);
                context.Response.OutputStream.Write(htmlData, 0, htmlData.Length);
                context.Response.Close();

                IAsyncResult result = httpListener.BeginGetContext(new AsyncCallback(WebRequest), httpListener);
            }
            catch (Exception e)
            {
                if (ReportError != null)
                {
                    ReportError("Sorry to give you bad news, but something's gone wrong.\nI've put some extra information into the clipboard - if you send it to Tejar, he'll do his best to help you.", e.ToString());
                }
                else
                {
                    MessageBox.Show("Sorry to give you bad news, but something's gone wrong.\nPlease report the following text to Tejar:\n" + e.ToString());
                }
            }
        }
    }
}
