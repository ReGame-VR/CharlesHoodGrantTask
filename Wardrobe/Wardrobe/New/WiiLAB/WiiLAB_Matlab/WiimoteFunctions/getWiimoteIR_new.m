function [X Y] = getWiimoteIR_new (source) 
% usage:    getWiimoteIR (source)
% purpose:  Gets the IR values from the wiimote for a given source
%           (1,2,3,4) or 0 for the midpoint
% return:	Returns a two element array with the following entries
%           index:	  1 2 
%		element:	[ X Y ]
% note:	the values will be X=0 and Y=0 if the source is not found

global wiimote;

% update the wiimote state values
wiimote.GetIRState();

% assign outputs
switch (source)
    case 0
        if(wiimote.IR.midpoint.found)
            X = 300 - wiimote.IR.midpoint.X * 300;
            Y = 300 - wiimote.IR.midpoint.Y * 300;
        else
            X = 0;
            Y = 0;
        end            
    case 1
        if (wiimote.IR.source1.found)
            X = wiimote.IR.source1.X;
            Y = wiimote.IR.source1.Y;
        else
            X = 0;
            Y = 0;
        end            
    case 2
        if (wiimote.IR.source1.found)
            X = wiimote.IR.source2.X;
            Y = wiimote.IR.source2.Y;
        else
            X = 0;
            Y = 0;
        end
    case 3
        if (wiimote.IR.source1.found)
            X = wiimote.IR.source3.X;
            Y = wiimote.IR.source3.Y;
        else
            X = 0;
            Y = 0;
        end
    case 4
        if (wiimote.IR.source1.found)
            X = wiimote.IR.source4.X;
            Y = wiimote.IR.source4.Y;
        else
            X = 0;
            Y = 0;
        end
    otherwise
        % Bad input
        error('Bad input to getWiimoteIR, "source" must be 1, 2, 3, 4 or 0 (for midpoint).');
end
