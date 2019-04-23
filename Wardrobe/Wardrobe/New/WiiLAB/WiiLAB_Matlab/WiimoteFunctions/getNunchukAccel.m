function [X Y Z] = getNunchukAccel () 
% usage:    getNunchukAccel () 
% purpose:  Gets the Acceleration values from the wiimote nunchuk
% return:	Returns a three element array with the following entries
%           index:	  1 2 3
%		element:	[ X Y Z ]

global wiimote;

% update the wiimote state values
wiimote.GetNunchukAccelState();

% assign outputs
X = wiimote.NunchukAccel.X; 
Y = wiimote.NunchukAccel.Y;
Z = wiimote.NunchukAccel.Z;

end