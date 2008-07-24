using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    public class SecurityStatus
    {
        public enum Level
        {
            Isolated,
            LowSecOnly,
            LowSecShortcut,
            HighSec
        }

        static public bool IsSecure(float Security)
        {
            if (Security >= 0.45f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static public bool IsSecure(Level Security)
        {
            if (Security != Level.LowSecOnly)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static public Level Min(Level a, Level b)
        {
            int x = (int)a;
            int y = (int)b;

            return (Level)Math.Min(x, y);
        }
    }
}
