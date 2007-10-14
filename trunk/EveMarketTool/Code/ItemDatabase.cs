using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class ItemDatabase : EveExportReader
    {
        Dictionary<string, ItemType> itemTypesByName = new Dictionary<string, ItemType>();
        Dictionary<int, ItemType> itemTypesById = new Dictionary<int, ItemType>();

        public List<ItemType> AllItems
        {
            get
            {
                return new List<ItemType>(itemTypesById.Values);
            }
        }

        public ItemType GetItemType(string name)
        {
            if(itemTypesByName.ContainsKey(name))
                return itemTypesByName[name];
            else
            {
                Console.WriteLine("Unrecognised item name \"" + name + "\"");
                return null;
            }
        }

        public ItemType GetItemType(int id)
        {
            if (itemTypesById.ContainsKey(id))
                return itemTypesById[id];
            else
            {
                Console.WriteLine("Unrecognised item id " + id);
                return null;
            }
        }

        public ItemDatabase()
        {
            ReadFromResource("Data.dbo_invTypes.csv");
        }

        public ItemDatabase(string fullPath)
        {
            ReadFromFullPath(fullPath);
        }

        protected override void InterpretRow(Dictionary<string, string> fields)
        {
            int id = ParseId(fields["typeID"]);
            string name = fields["typeName"];
            float volume = ParseNumber(fields["volume"]);
            ItemType t = new ItemType(id, name, volume);
            if (!itemTypesByName.ContainsKey(t.Name))
            {
                itemTypesByName.Add(t.Name, t);
                itemTypesById.Add(t.Id, t);
            }
            else
                Console.WriteLine("Warning: duplicate item " + t.Name);
        }
    }
}
