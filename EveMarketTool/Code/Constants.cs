using System;
using System.Collections.Generic;
using System.Text;

namespace EveMarketTool
{
    // Temporary used for tax calculation -- sorry I fail at life. -terry
    public class Variables {
        public static float Total_Sales = 0f;
    }

    public class Constants
    {
        // public readonly static string EveVersionName = "Dominion";
        public readonly static bool IsBetaVersion = false;

        public readonly static int dbg_trips_arg = 2;

        public readonly static string BetaTooltipMessage =
            "This is a beta version of NavBot. It may crash, be inaccurate, or not work at all. Use at your own ISK risk!";

        public readonly static string Revision = "L (Release)";
        public readonly static string Credits = 
            "NavBot was written by Tejar. Contributions have been made generously by: Mark O'Connor, " +
            "Michael Votteler, Bart Seresia, Michael Mitchell, munged92, Michael Hollnbuchner, ok123jump, " +
            "jlstrobel (John Strobel), rigbyd, Terracarbon (Terry Winfrey), Papa Yoru (Tony Jackson) and others.";
    }
}
