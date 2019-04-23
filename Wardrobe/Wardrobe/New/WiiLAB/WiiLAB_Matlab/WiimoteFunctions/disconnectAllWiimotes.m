function disconnectAllWiimotes()
% usage:      	disconnectAllWiimotes ()
% purpose:		Attempts to disconnect all the wiimotes connected
%                 This function was added to allow the user to free
%			up all of the Wiimotes paired to the computer.
%			If a Wiimote is not disconnected properly after
%			being used the user may not be able to reconnect
%			to it from a different program. This function will
%			disconnect the Wiimotes allowing them to be reconnected

global wiimote;

% Disconnects all the wiimotes on this computer
wiimote.DisconnectAllWiimotes();

end
