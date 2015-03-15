Thank-you for your purchase of the NavBot-class trade assistant. This short guide will help you install the unit and connect it with your Pod's systems.

## Installing NavBot L ##
To install, simply run "NavBot-L.exe" and choose an installation path. This installer and version should automatically ask you to elevate administrator.

**Note: This installer will not create a Start Menu item at this time. This version will also use port 80 as default to address the current in game browser trust issue. If this issue is ever fixed and you wish to use a different port, you can now change this information in NavBot.exe.config**

## Installing NavBot J and setting it to run as Administrator ##
To install, extract all the files from the zip and then right-click on setup-run-as-administrator.exe choose "Run as Administrator". NavBot should automatically detect the EVE directories that link you to your Pod and will run in the system tray (bottom-right).

**All Versions of NavBot:**

WARNING: NavBot must always be run as the administrator. This is a bit awkward, but you can set it up as follows:
  1. Search for NavBot.exe (in Vista+ you can do this by clicking on Start, typing "NavBot.exe" and waiting)
  1. Right-click on the NavBot.exe result and open its properties.
  1. Check that it's the version you just installed (the date should tell you)
  1. Go to the Compatibility tab
  1. Put at tick in the "Run as Administrator" check box
  1. Click OK
Now whenever you run NavBot, it will run as the Administrator. If you don't do this, it won't work. We're sorry! If you know some C# and want to fix this, go ahead and grab the source from this site and submit a patch. We're very friendly!

 Important note to Skype Users 
To use NavBot with Skype you will need to close Skype, run Navbot and reopen Skype to fix NavBot's error that the port is in use.

## Your First Time with NavBot ##
Once NavBot is running in the taskbar, go into EVE and open the in-game browser. Either press Ctrl-V or type "http://localhost" into the address bar, and press enter. This will connect you to NavBot.

To get the most out of NavBot, you need to make it a "trusted site". NavBot itself gives you links to do this, but sometimes the EVE browser seems to forget. You might have to open the list of trusted sites manually from the menu bar and check that http://localhost is listed as trusted.

Since this is your first time, let's take it slow. Ask NavBot to show you a list of market reports you already have (3rd option). If you have any reports, tell NavBot to remove them all - this places them in an Archive folder in case you want them again in the future.

Now, open the market in EVE and find "Reports" in the list of trade items. Select it, then click on the "Export Data" button.

Now go back to NavBot and ask to see how you can make money / start trading (1st option).
NavBot will ask you how much ISK you have available and how much cargo space you have available. Fill in these values and press "Enter".

Now for the best bit - the top trade routes! NavBot finds three kinds of routes:
  1. The best (most profit per warp) single trade trip you can make, starting from your current system. This is what you'll usually use; it tells you how to maximise the amount of isk you're earning per hour.
  1. The best (most profit per warp) single trade trip between any two systems that NavBot can find. This gives you an idea of how much profit is available in the regions you've exported, and can help you decide whether to stay where you are or go to a different system to start trading there.
  1. The largest profit in total to be found between any two systems. This is perfect if you just want to stick the autopilot on, sit back and relax.
For each type of route, NavBot shows two routes - one with systems that start and end in secure space (>0.5) and one with systems that may start or end in insecure space (<0.5).

Picking a route is easy - you can right-click on any of the links and choose to autopilot to them or even dock directly at the correct station, all from NavBot's report screen. The type and number of items to buy are shown too!

Finally, after each trip remember to re-export the market data for any item you've just sold, and to update your ISK in NavBot - the routes you get are only as fresh as the data you give it!

Happy trading!