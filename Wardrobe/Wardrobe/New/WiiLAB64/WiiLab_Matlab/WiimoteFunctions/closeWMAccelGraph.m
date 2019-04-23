function closeWMAccelGraph()
%  usage:    closeWMAccelGraph()
%  purpose:  signifies that the user is done plotting data on the open
%            Wiimote acceleration plot

global wiimote;

% close graph
CloseWmAccelGraph(wiimote);

end