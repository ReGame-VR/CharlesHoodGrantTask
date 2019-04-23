function setWiimoteLEDs (led1, led2, led3, led4) 
% usage:    setWiimoteLEDs (led1, led2, led3, led4) 
% purpose:  Sets the LEDs on the wiimote
%           led1/2/3/4 are bool values (or 1/0 int) true to turn on rumble, false to turn
%           it off

global wiimote;

% Set the LEDs
wiimote.LEDs =  [led1 led2 led3 led4];

end