# KRASH
KRASH - Kerbal Ramification Artifical Simulation Hub (simulation mod for KSP)

This mod includes version checking using MiniAVC (http://forum.kerbalspaceprogram.com/threads/79745)
If you opt-in, it will use the internet to check whether there is a new version available. Data is only
read from the internet and no personal information is sent. For a more comprehensive version checking 
experience, please download the full KSP-AVC plugin (http://forum.kerbalspaceprogram.com/threads/79745">KSP-AVC Plugin)

KRASH lets you launch flights from the VAB\SPH, and when the flight is over, it restores the game to the exact state that 
it was in before the simulation started. It handles restoring the state of any mod which stores its data in the vanilla 
persistence file (basically any of them worth their salt), and even handles it if you exit the game during a simulation.

Instructions

Configurations

There are 10 pre-defined configurations:

Easy		Fairly low costs.  There are setup costs and per-minute costs, both for the number of parts and weight of the vessel
Normal		Reasonable costs
Moderate	Somewhat more expensive
Hard		Even more expensive
Easy-Percentage	Same as easy, except that instead of using the number of parts and weight, it uses a percentage of the cost of the vessel for the setup costs, and then a smaller percentage for each minute
Normal-Percentage
Moderate-Percentage
Hard-Percentage

For both type listed above, there is also a Custom setting

In addition, you can create a customized config.  See the following image for the configuration screen:

In the left column is a list of the available configs to use.  An asterick indicates one of the predefined settings and can't be changed.  Click on 
the setting you would like to see, the values will be shown on the right side

On the right half, the top shows the config name and the buttons:
Save	Active only when the name doesn't begin with an asterick.  Will save the current config and make it active
Accept	Accepts the current config as the active config for the current game
Reset	Resets change to the current config
Cancel	Cancels the changes and closes the screen
Delete	Active only when name refers to existing custom config.  Will delete that config

In the right half, there are two columns;  The left column shows the costs for the different 
types:  Flat, Per-part, Per-ton and Percentage, along with the Atmospheric Multipler

The right colum has the following:

Termination options if no data at the specified location(ie:  Terminate at SOI, if  you enter an SOI
and haven't received any science data from it, the sim will terminate)

Miscellaneous Options.  The max sim cost is merely a default, and can be overridden when  you start the sim

Note that the configuration window can be shown both in the Spacecenter view and in the editor, whether you are in
a career game or not.  The costs will only apply if you are in a career game.


In the VAB/SPH, load the ship you want to simulate
Click the SIM button, you will see the following screen:
![](http://i.imgur.com/gaEXswY.png)

The buttons at the top select the starting location, and are as follows:

Kerbin - The homeworld of the game.  If you are running a mod which changes the homeworld, it will be shown here
Landed - Start sim landed on any planetary body
Orbit Selection - Start sim in an orbit around any planetary body

In career mode, if you haven't reached a planetary body, you won't be able to start the sim there

Starting sim at Kerbin
You will be able to start the sim either at the Runway or the Launchpad.  Selected which one you want,see this screen:
![](http://i.imgur.com/UXLcFe3.png)

Starting sim Landed
There is a row of buttons which will allow you to filter the displayed planetary bodies by either planets, moons, or all.
Select the planetary body you want to start the sim at from the displayed list.  Also, 
enter an altitude where the vessel should be started at.  You can also enter the Latitude 
and Longitude for the starting location.  It will start there, and very 
slowly descend until it touches the ground.  See the following screen for details:
![](http://i.imgur.com/UXLcFe3.png)

Starting sim in Orbit
There is a row of buttons which will allow you to filter the displayed planetary bodies by either planets, moons, or all.
Select the planetary body you want to start the sim at from the displayed list.  Also, 
enter an altitude where the vessel should be started at.  Note that when you select a body, the
altitude will be automatically adjusted to be just outside the atmosphere if it has one, or
at an altitude of 10000m.  You can override it with whatever values you want.
![](http://i.imgur.com/QWgr5Xc.png)

Click the big green button to start the simulation

IMPORTANT NOTE:  If you are running RemoteTech, be sure that ALL your antennas are set to start retracted,
otherwise they will be ripped off during the teleportation process.

During the simulation, everything works normally, except you can't go to the Space Center.  Hitting the Escape key
will bring up the Simulation menu, see the following screenshot:
![](http://i.imgur.com/EWNWtqc.png)

At this menu, you can either restart the simulation, or terminate the simulation and return to the editor.

In the event of either a game crash during the simulation, or exiting the program, restarting the game will restore 
the game to the point just before you started the simulation.

When the simulation is terminated, you will see the usual flight history dialog before the final revert.

When playing in career mode, there is some additional functionality:

1.  Simulations now cost money (ie:  computer time = money).  The cost of the simulation
	consists of several parts:  A flat cost, a per-part cost and a per-ton cost.  These costs
	are calculated for an initial setup cost, and a per-minute cost.  The costs for the simulation
	differs based on the level of game you are playing.  Easy, Normal, Moderate, Hard and Custom.
	The costs are defined in the KRASH.cfg file, and can be changed if desired
	The estimated simulation costs are shown while you are setting up the simulation in the
	lower right-hand column.
	You can also enter an upper limit for the simulation costs.  When the simulation costs equal
	the specified limit, the simulation will end.

2.	Simulations will terminate if one of the following situations happens:

			Simulation terminated due to lack of funds
			Simulation terminated due to cost limit reached
			Simulation terminated due to lack of science data
			Simulation terminated due to entering unknown SOI
			Simulation terminated due to landing on unknown body
			Simulation terminated due to vessel destruction