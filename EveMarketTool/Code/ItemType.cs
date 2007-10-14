using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class ItemType
    {
        private string name;
        public string Name
        {
            get { return name; }
        }

        private int id;
        public int Id
        {
            get { return id; }
        }

        private float volume;
        public float Volume
        {
            get { return volume; }
        }

        public ItemType(int id, string name, float volume)
        {
            this.id = id;
            this.name = name;
            this.volume = volume;
        }
    }
}
