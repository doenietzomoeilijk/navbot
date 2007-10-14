using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace EveMarketTool.Tests
{
    [TestFixture]
    public class CSVFileReaderTest
    {
        [Test]
        public void ReadHeadings()
        {
            CSVFileReaderTestWrapper reader = new CSVFileReaderTestWrapper("CSVFileReaderSimpleTestInput.csv");
            Assert.AreEqual(1, reader.Fields.Count);
            Dictionary<string, string> fields = reader.Fields[0];
            Assert.IsTrue(fields.ContainsKey("Heading1"));
            Assert.IsTrue(fields.ContainsKey("Heading2"));
            Assert.IsTrue(fields.ContainsKey("Heading with space"));
        }

        [Test]
        public void ReadSimpleItems()
        {
            CSVFileReaderTestWrapper reader = new CSVFileReaderTestWrapper("CSVFileReaderSimpleTestInput.csv");
            Assert.AreEqual(1, reader.Fields.Count);
            Dictionary<string, string> fields = reader.Fields[0];
            Assert.AreEqual("Item 1", fields["Heading1"]);
            Assert.AreEqual("Item 2", fields["Heading2"]);
            Assert.AreEqual("Item 3", fields["Heading with space"]);
        }

/*        [Test]
        public void ReadQuotedItems()
        {
            CSVFileReaderTestWrapper reader = new CSVFileReaderTestWrapper("CSVFileReaderQuotedTestInput.csv");
            Assert.AreEqual(1, reader.Fields.Count);
            Dictionary<string, string> fields = reader.Fields[0];
            Assert.AreEqual("\"double quoted item, with comma\"", fields["Double"]);
            Assert.AreEqual("'single quoted item, with comma'", fields["Single"]);
        }
 * */
    }

    class CSVFileReaderTestWrapper : CSVFileReader
    {
        public List<Dictionary<string, string>> Fields = new List<Dictionary<string, string>>();

        public CSVFileReaderTestWrapper(string inputFile)
        {
            ReadFromFullPath("C:\\Dokumente und Einstellungen\\Mark\\Eigene Dateien\\My Code\\EveMarketTool\\EveMarketTool\\Tests\\" + inputFile);
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            Fields.Add(new Dictionary<string, string>(fields));
        }
    }
}
