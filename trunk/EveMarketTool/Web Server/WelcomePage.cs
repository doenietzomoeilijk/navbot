using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;

namespace EveMarketTool
{
    public class WelcomePage : Page
    {
        string welcomeWithoutTrust;
        string welcomeWithTrust;

        public WelcomePage()
        {
            welcomeWithoutTrust = ReadFromResource("Web_Server.WelcomeWithoutTrust.html");
            welcomeWithTrust = ReadFromResource("Web_Server.WelcomeWithTrust.html");
        }

        public override string Name()
        {
            return "Welcome";
        }

        public override string Render(string systemName, string charName, string charId, NameValueCollection headers, NameValueCollection query)
        {
            if (charName == null)
                return ReplaceVariables(welcomeWithoutTrust, systemName, charName, charId);
            else
                return ReplaceVariables(welcomeWithTrust, systemName, charName, charId);
        }
    }
}
