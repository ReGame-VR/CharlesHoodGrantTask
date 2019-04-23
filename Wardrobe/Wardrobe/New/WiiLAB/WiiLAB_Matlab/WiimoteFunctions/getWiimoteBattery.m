function B = getWiimoteBattery () 
% usage:    getWiimoteBattery ()
% purpose:  Gets the Percentage of Battery remaining on the Wiimote
% return:	a double value representing the percentage of the battery remaining

global wiimote;

% update the wiimote state values
wiimote.GetBatteryState();

% assign outputs
B = wiimote.Battery;

end