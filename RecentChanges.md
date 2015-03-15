# [r13](https://code.google.com/p/navbot/source/detail?r=13), [r14](https://code.google.com/p/navbot/source/detail?r=14), [r15](https://code.google.com/p/navbot/source/detail?r=15) #

Features added:
  * Trades are calculated on a station to station basis.  This means we only look at items between those stations which are profitable, and we also look to fill the hold with multiple item types, in order of profitability per cargo unit.
  * Security Status - Two routes are calculated for each starting and destination systems.  A fastest route, and a secure route.  If the secure route has the same # of jumps as the fastest route - it is a high-sec trip.  If the secure route is longer than the fastest route, it is a low-sec shortcut.  If the secure route is not traversable - it is low-sec only.  Each of these are displayed in the browser in the number of jumps area.  Green for # of jumps is high-sec, red is low-sec.  Occasionally you will see something like (8 jumps)/(3 jumps) - where the 8 is green and the 3 is red.  This is indicating that the route would take 8 jumps through high-sec, and 3 through lowsec.  Profit per warp is calculated for each as well.
  * Tax information is calcuated, and profit figured based on amount of tax.
  * Added profit margin notation.
  * Added support for handling buy orders with a range.  This will potentially find orders that can be purchased in one system, and brought a shorter distance than it would normally take to get to the system the buy order is actually in.  Combined with looking at trades for multiple items, this has the potential to find some hidden gems of trade routes.

Enhancements:
  * The map is saved between requests.  This prevents the program from having to recalculate the routing from one point to another.
  * Modified the trip finder to return a variable number of trips.  Currently, it is displaying 2 trips of each type, however that has complicated the display a bit.  This is something that should be fixed in the next pass of changes.

Bug Fixes:
  * Minimum order amounts are taken into consideration.  If a trade cannot meet a minimum order requirement, it is thrown out.

Known issues:
  * The current design of finding all trades between stations, and taking buy order ranges int account leads to the map being built up for most routes between systems in a region.  The first time a search is done with market data from a region, and especially if multiple regions are involved, NavBot is going to find distances to/from each system affected.  This causes a delay in processing the information.  On subsequent passes however, refreshing the data and recalculating the routes only takes 10-20 seconds - much faster than in prior versions.  The fact that this data is saved does lead NavBot to hold onto more memory than previous versions.