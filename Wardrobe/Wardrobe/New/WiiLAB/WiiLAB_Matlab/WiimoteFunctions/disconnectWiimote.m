function disconnectWiimote()
% usage:      	disconnectWiimote ()
% purpose: 		Disconnects the wiimote
% note:       	This should be called when you are done using the Wiimote

global wiimote;

% Disconnect the wiimote
wiimote.Disconnect();

end
