function setWiimoteRumble (on) 
% usage:    setWiimoteRumble (on) 
% purpose:  Sets the rumble on the wiimote
%           on is a bool (or 1/0 int) true to turn on rumble, false to turn
%           it off

global wiimote;

% Set the rumble
wiimote.SetRumble(on);

end