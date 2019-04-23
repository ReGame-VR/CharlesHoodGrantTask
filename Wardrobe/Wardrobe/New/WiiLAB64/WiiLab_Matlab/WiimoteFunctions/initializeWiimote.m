function initializeWiimote () 
%	usage:      initializeWiimote (wiimote)
%	purpose:	Takes care of initializing and connectign the wiimote


% Initialize wiimote one
global wiimote;

wiimote = Wiimote();
wiimote.Connect();

% If there was a problem connecting to the wiimote
% Specifically, There are wiimotes connected to the computer but they are
% connected to another object
if(wiimote.isConnected == 0)
    
    % Disconnect all the wiimotes on this computer
    disp('Removing all connected wiimotes...');
    wiimote.DisconnectAllWiimotes();
    % Retry the connection
    disp('Attempting to connect...');
    wiimote = Wiimote();
    wiimote.Connect();
    
end

end
