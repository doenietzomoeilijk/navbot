using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace EveMarketTool.Tests.Mock_Objects
{
    class MockPage : Page
    {
        public string SystemName;
        public string CharName;
        public string CharId;
        public NameValueCollection Headers;
        public NameValueCollection Query;

        public override string Name()
        {
            return "test";
        }

        public override string Render(string systemName, string charName, string charId, NameValueCollection headers, NameValueCollection query)
        {
            SystemName = systemName;
            CharName = charName;
            CharId = charId;
            Headers = headers;
            Query = query;
            return "<html><body>test page</body></html>";
        }
    }
}
