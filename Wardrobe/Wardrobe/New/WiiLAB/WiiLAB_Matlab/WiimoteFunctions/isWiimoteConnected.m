function [isConnected] = isWiimoteConnected()
% usage:      	isNunchukConnected()
% purpose:		Checks if the wiimote is connected
% return:		boolean value indicating if the Wiimote is connected

global wiimote;

% Disconnect the wiimote
isConnected = wiimote.isConnected;

end
