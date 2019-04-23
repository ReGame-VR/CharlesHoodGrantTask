function [C Z] = getNunchukButtons () 
% usage:    getNunchukButtons () 
% purpose:  Gets the Button values from the wiimote nunchuk (true if they are
%           pressed - false if they are not)
% return:	Returns a  two dimensional array with the following entries
%		index:  	1 2
%		element:  [ C Z ]

global wiimote;

% update the wiimote state values
wiimote.GetNunchukButtonsState();

% assign outputs
C = wiimote.NunchukButtons.C;
Z = wiimote.NunchukButtons.Z;

end