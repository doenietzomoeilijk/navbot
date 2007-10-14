using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EveMarketTool;
using System.Threading;

namespace NavBot
{
    public partial class InitialForm : Form
    {
        PageServer server = null;

        public InitialForm()
        {
            InitializeComponent();
            notifyIcon.BalloonTipTitle = "NavBot rev. " + Constants.Revision;
            Text = "NavBot rev. " + Constants.Revision;

            string path = Application.UserAppDataRegistry.GetValue("DefaultEvePath") as string;
            if (path != null)
            {
                eveDirectory.Text = path;
            }

            if (!PathLooksValid())
            {
                string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\EVE\\logs\\Marketlogs";
                if (PathLooksValid(defaultPath))
                    eveDirectory.Text = defaultPath;
            }
        }

        void TryToStartNavBot()
        {
            try
            {
                server = new PageServer();
                server.ReportError += new PageServer.ErrorMessageHandler(ReportError);
                server.AddPage(new WelcomePage());
                server.AddPage(new SearchPage(new TradeFinderFactory(eveDirectory.Text)));
                server.AddPage(new ReportsPage(new TradeFinderFactory(eveDirectory.Text)));

                try
                {
                    Clipboard.SetText("http://localhost:9999");
                }
                catch (Exception)
                {
                }

                notifyIcon.Visible = true;
                Hide();
                notifyIcon.ShowBalloonTip(7000);
            }
            catch (PlatformNotSupportedException e)
            {
                WindowState = FormWindowState.Normal;
                if (server != null)
                {
                    server.Close();
                }
                ReportError("Sorry, NavBot requires Windows XP SP2, Windows Vista or Windows Server 2003 to run.\nNo, really, I *am* sorry, but Microsoft simply doesn't support the HttpListener class on anything else.", e.ToString());
            }
            catch (Exception e)
            {
                WindowState = FormWindowState.Normal;
                if (server != null)
                {
                    server.Close();
                    ReportError("Are you sure EVE stores your market logs in \"" + eveDirectory.Text + "\"?\nIf so, then something else has gone wrong.\nI've put some extra error information in your clipboard - if you send it to Tejar then he'll do his best to help you.", e.ToString());
                }
                else
                {
                    ReportError("Is another copy of NavBot already running? If not, try restarting or send an EVE mail to Tejar for help. \nI've put some extra error information in your clipboard that you can send to him.", e.ToString());
                }
            }
        }

        private void notifyIcon_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me != null)
            {
                if (me.Button == MouseButtons.Left)
                {
                    notifyIcon.ShowBalloonTip(5000);
                    try
                    {
                        Clipboard.SetText("http://localhost:9999");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void InitialForm_ResizeEnd(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void iconMenuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void run_Click(object sender, EventArgs eventArgs)
        {
            TryToStartNavBot();
        }

        private void browseFolderButton_Click(object sender, EventArgs e)
        {
            if (browseFolderDialog.ShowDialog() == DialogResult.OK)
            {
                eveDirectory.Text = browseFolderDialog.SelectedPath;
            }
        }

        private void eveDirectory_TextChanged(object sender, EventArgs e)
        {
            if (PathLooksValid())
            {
                runButton.Enabled = true;
                Application.UserAppDataRegistry.SetValue("DefaultEvePath", eveDirectory.Text);
            }
            else
            {
                runButton.Enabled = false;
            }
        }

        Mutex myMutex = new Mutex();
        string messageText;
        string errorText;
        void ReportError(string text, string extra)
        {
            myMutex.WaitOne();
            messageText = text;
            errorText = extra;
            myMutex.ReleaseMutex();
        }

        private void checkForErrorsTimer_Tick(object sender, EventArgs e)
        {
            myMutex.WaitOne();
            string text = messageText;
            string error = errorText;
            messageText = null;
            errorText = null;
            myMutex.ReleaseMutex();

            if (text != null && error != null)
            {
                checkForErrorsTimer.Enabled = false;
                try
                {
                    Clipboard.SetText(error);
                }
                catch (Exception)
                {
                    if(errorText != "")
                        text += "\nI also tried to put the following into the clipborad, but couldn't:\n" + errorText;
                }
                MessageBox.Show(text);
                checkForErrorsTimer.Enabled = true;
            }
        }

        private void InitialForm_Load(object sender, EventArgs e)
        {
            if (PathLooksValid())
            {
                TryToStartNavBot();
            }
            else
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private bool PathLooksValid(string path)
        {
            return path != null && Directory.Exists(path) && path.Contains("\\Marketlogs");
        }

        private bool PathLooksValid()
        {
            return PathLooksValid(eveDirectory.Text);
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (server != null)
                server.Close();

            Show();
            WindowState = FormWindowState.Normal;
        }
    }
}