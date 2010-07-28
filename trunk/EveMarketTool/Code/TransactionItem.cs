using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EveMarketTool
{
    public class TransactionItem
    {
        private Trade tradeItem;
        private int quantity;
       
        public Trade TradeItem
        {
            get
            {
                return tradeItem;
            }
            set
            {
                tradeItem = value;
            }
        }

        public int Quantity
        {
            get
            {
                if (tradeItem.Quantity < quantity)
                {
                    quantity = tradeItem.Quantity;
                }
                return quantity;
            }
            set
            {
                quantity = Math.Min(value, tradeItem.Quantity);
            }
        }

        public float UnitPrice
        {
            get
            {
                if (tradeItem != null)
                {
                    return tradeItem.UnitPrice;
                }
                else
                {
                    return 0.0f;
                }
            }
        }

        public int MinQuantity
        {
            get
            {
                if (tradeItem != null)
                {
                    return tradeItem.MinQuantity;
                }
                else
                {
                    return 0;
                }
            }
        }

        public ItemType Type
        {
            get
            {
                if (tradeItem != null)
                {
                    return tradeItem.Type;
                }
                else
                {
                    return null;
                }
            }
        }

        public TransactionItem(Trade tradeItem) : this(tradeItem, tradeItem.Quantity)
        {
        }

        public TransactionItem(Trade tradeItem, int quantity)
        {
            this.tradeItem = tradeItem;
            this.Quantity = quantity;
        }

        public override string ToString()
        {
            Variables.Total_Sales += Quantity * UnitPrice;
            string output = String.Format("<tr><td><font size='$fontsz_tiny' face='$fontname'><a href=\"javascript:CCPEVE.showInfo({0})\">{1}</a>", Type.Id, Type.Name) +
                     String.Format(CultureInfo.InvariantCulture + "</font></td><td><font size='$fontsz_tiny' face='$fontname'>{0}</font></td><td><font size='$fontsz_tiny' face='$fontname'>{1}</font></td><td><font size='$fontsz_tiny' face='$fontname'>{2:0.##}</font></td></tr>", Quantity, UnitPrice, Quantity * UnitPrice);
            // lazy sorry
            output = output.Replace("$fontname", ConfigurationSettings.AppSettings["FontName"]);
            output = output.Replace("$fontsz_tiny", ConfigurationSettings.AppSettings["FontSize_Tiny"]);

            return output;
        }
    }
}
