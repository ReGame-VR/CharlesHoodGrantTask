function [X1 Y1 X2 Y2 X3 Y3 X4 Y4] = getWiimoteIR () 
% usage:    getWiimoteIR ()
% purpose:  Gets the IR values from the wiimote, the Wiimote can track up to 4 sources
% return:	Returns an eight element array with the following entries
%           index:	  1  2  3  4  5  6  7  8
%		element:	[ X1 Y1 X2 Y2 X3 Y3 X4 Y4 ]

global wiimote;

% update the wiimote state values
wiimote.GetIRState();

% assign outputs
X1 = wiimote.IR.source1.X;
Y1 = wiimote.IR.source1.Y;
X2 = wiimote.IR.source2.X;
Y2 = wiimote.IR.source2.Y;
X3 = wiimote.IR.source3.X;
Y3 = wiimote.IR.source3.Y;
X4 = wiimote.IR.source4.X;
Y4 = wiimote.IR.source4.Y;

end