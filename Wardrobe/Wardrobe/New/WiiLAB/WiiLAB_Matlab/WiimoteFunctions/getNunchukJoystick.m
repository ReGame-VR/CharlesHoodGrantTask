function [X Y] = getNunchukJoystick () 
% usage:    getNunchukJoystick () 
% purpose:  Gets the Joystick values from the wiimote nunchuk
% return:	Returns a two element array with the following entries
%           index:	  1 2 
%		element:	[ X Y ]

global wiimote;

% update the wiimote state values
wiimote.GetNunchukJoystickState();

% assign outputs
X = wiimote.NunchukJoystick.X;
Y = wiimote.NunchukJoystick.Y;

end