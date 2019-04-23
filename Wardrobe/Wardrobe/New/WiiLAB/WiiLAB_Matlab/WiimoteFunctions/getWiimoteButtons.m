function [A B UP RIGHT DOWN LEFT PLUS MINUS HOME ONE TWO] = getWiimoteButtons () 
% usage:    getWiimoteButtons ()
% purpose:  Gets the Button values from the wiimote (true if they are
%           pressed - false if they are not)
% return:	Returns an 11 element array with the following entries
%           index:	  1 2 3    4    5    6    7     8    9    10  11
%		element:	[ A B UP RIGHT DOWN LEFT PLUS MINUS HOME ONE TWO ]

global wiimote;

% update the wiimote state values
wiimote.GetButtonsState();

% assign outputs
A = wiimote.Buttons.A;
B = wiimote.Buttons.B;
UP = wiimote.Buttons.Up;
RIGHT = wiimote.Buttons.Right;
DOWN = wiimote.Buttons.Down;
LEFT = wiimote.Buttons.Left;
PLUS = wiimote.Buttons.Plus;
MINUS = wiimote.Buttons.Minus;
HOME = wiimote.Buttons.Home;
ONE = wiimote.Buttons.One;
TWO = wiimote.Buttons.Two;

end