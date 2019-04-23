function [X Y Z] = getWiimoteAccel () 
% usage:    getWiimoteAccel ()
% purpose:  Gets the Acceleration values from the wiimote
% return:	Returns a three element array with the following entries
%           index:	  1 2 3
%		element:	[ X Y Z ]

global wiimote;

% update the wiimote state values
wiimote.GetAccelState();

% assign outputs
X = wiimote.Accel.X;
Y = wiimote.Accel.Y;
Z = wiimote.Accel.Z;

end