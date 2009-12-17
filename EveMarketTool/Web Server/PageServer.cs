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
            HttpListenerContext context = null;

            try
            {
                context = httpListener.EndGetContext(ar);
                //navbot-61 - Header Keys Changed in Dominion
                string systemName = context.Request.Headers["EVE_SOLARSYSTEMNAME"];
                string charName = context.Request.Headers["EVE_CHARNAME"];
                string charId = context.Request.Headers["EVE_CHARID"];
                /* Other available header keys in Dominion
                 * EVE_CHARNAME: username
                   EVE_STATIONNAME: Foobar - Center for Advanced Studies School
                   EVE_CONSTELLATIONNAME: Anwyns
                   EVE_CORPNAME: The Corp
                   EVE_CHARID: 1234567890
                   EVE_CORPID: 1234567
                   EVE_SOLARSYSTEMNAME: Foobar
                   EVE_REGIONNAME: Verge Vendor
                   EVE_TRUSTED: Yes
                   EVE_SERVERIP: 87.237.38.200:26000
                 */

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
                    htmlOutput = "<html><body>Oops - I seem to have lost my mind. Please report this to http://code.google.com/p/navbot/issues at once!</body></html>";
                }

                byte[] htmlData = System.Text.UTF8Encoding.UTF8.GetBytes(htmlOutput);
                //navbot-61 New Browser appears to be based on Mozilla.  Needs to have Content Type and Length set on the output HTTP Header
                context.Response.ContentType = "text/html; charset=utf-8";
                context.Response.ContentLength64 = htmlData.Length;
                context.Response.OutputStream.Write(htmlData, 0, htmlData.Length);

            }
            catch (HttpListenerException httpEx)
            {
                //--navbot-61 Will happen if the user impatient and clicks the link twice before the page loads--
                Console.Write(httpEx);
            }
            catch (Exception e)
            {
                if (ReportError != null)
                {
                    ReportError("Sorry to give you bad news, but something's gone wrong.\nI've put some extra information into the clipboard - if you post it to http://code.google.com/p/navbot/issues, someone may be able to help you.", e.ToString());
                }
                else
                {
                    MessageBox.Show("Sorry to give you bad news, but something's gone wrong.\nPlease report the following text to http://code.google.com/p/navbot/issues:\n" + e.ToString());
                }
            }
            finally
            {
                //navbot-61 Found that with errors, the output stream was not getting closed.
                try
                {
                    context.Response.OutputStream.Close();
                }
                catch (Exception e2)
                {
                    Console.Write(e2);
                }
                IAsyncResult result = httpListener.BeginGetContext(new AsyncCallback(WebRequest), httpListener);
            }
        }
    }
}
