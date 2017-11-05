API Documentation for KRASH

Include the file KRASHWrapper.cs in your project.
The KRASHWrapper.cs file is provided as-is and is not to be modified other than to update
the namespace. Should further modification be made, no support will be provided by the author,
linuxgurugamer.

If you wish to use your own logging functions, you can replace the function Infolog with a call
to your functions

The following functions are available to interface with KRASH:


KRASHAvailable returns true or false to indicate whether KRASH is available or not
bool KRASHAvailable

SetoverrideNode will allow you to override the default node from the KRASH.cfg file.  The KRASH.cfg file contains the following nodes:
Easy, Normal, Moderate, Hard, Custom

You can use ModuleManager to add additional nodes to the file and specify which one you want to use with this funciton

bool SetOverrideNode(string node)

doTerminateSim will terminate the simulation
bool doTerminateSim(string msg)

SetAllCosts will set all the costs, both flat, and per minute.  This function actually calls both SetSetupCosts() and SetPerMinCosts(), it is
provided here for convenience
bool SetAllCosts(float flatSetupCost, float perPartSetupCost, float perTonSetupCost, float flatPerMinCost, float perPartPerMinCost, float perTonPerMinCost)

SetSetupCosts will set the setup costs for the sim
bool SetSetupCosts(float flatSetupCost, float perPartSetupCost, float perTonSetupCost)

SetPerMinCost will set the costs per minute for the sim
bool SetPerMinCost(float flatPerMinCost, float perPartPerMinCost, float perTonPerMinCost)

SetFlastCosts will set the flat costs, both setup and per minute
bool SetFlatCosts(float flatSetupCost, float flatPerMinCost)

SetPerPartCosts will set the per part costs, both setup and per minute
bool SetPerPartCosts(float perPartSetupCost, float perPartPerMinCost)

SetPerTonCosts will set the per ton costs, both setup and per minute
bool SetPerTonCosts(float perTonSetupCost, float perTonPerMinCost)

addtoCosts will add the specified amount to the simulation cost
bool AddToCosts(float cost)

GetCurrentSimcosts will return the current costs of the simulation.
double GetCurrentSimCosts()

============================================================================

The following sets of functions will allow you to add callbacks to KRASH.  The
added callbacks will be called when the event happens.

******************************************
/* Adds a listener to the Sim Menu Event. When the initial KRASH menu is entered the method will 
 * be invoked with the Vessel; a double with 0 */
void AddSimMenuEvent (Action<Vessel, double> method)

/* Removes a listener from the Sim Menu Event */
void RemoveSimMenuEvent (Action<Vessel, double> method)

******************************************

/* Adds a listener to the Sim Start Event. When the simulation is started the method will 
 * be invoked with the Vessel; a double with 0 */
void AddSimStartEvent (Action<Vessel, double> method)

/* Removes a listener from the Sim Start Event */
void RemoveSimStartEvent (Action<Vessel, double> method)

******************************************

/* Adds a listener to the Sim Restart Event. When the simulation is restarted the method will 
 * be invoked with the Vessel; a double with the cost of the simulation to this point in time */
void AddSimRestartEvent (Action<Vessel, double> method)

/* Removes a listener from the Sim Restart Event */
void RemoveSimRestartEvent (Action<Vessel, double> method)

******************************************

/* Adds a listener to the Sim Termination Event. When the simulation is terminated the method will 
 * be invoked with the Vessel; a double with the total cost of the simulation */
void AddSimTerminationEvent (Action<Vessel, double> method)

/* Removes a listener from the Sim Termination Event */
void RemoveSimTerminationEvent (Action<Vessel, double> method)

******************************************

/* Adds a listener to the Sim Termination Event. When the simulation is terminated the method will 
 * be invoked with the Vessel; a double with the total cost of the simulation */
void AddSimTimedEvent (Action<Vessel, double> method)

/* Removes a listener from the Sim Termination Event */
void RemoveSimTimedEvent (Action<Vessel, double> method)
