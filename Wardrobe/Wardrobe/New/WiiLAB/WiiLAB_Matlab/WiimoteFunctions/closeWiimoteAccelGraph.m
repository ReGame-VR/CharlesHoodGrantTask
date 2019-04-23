function closeWiimoteAccelGraph()
% usage:     closeWMAccelGraph()
% purpose:   signifies that the user is done plotting data from the
%            Wiimote and closes the graph

global wiimote;

% close graph
wiimote.CloseWiimoteAccelGraph();
