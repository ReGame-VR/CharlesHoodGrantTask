function initializeWMAccelGraph()
%  usage:    initializeWMAccelGraph()
%  purpose:  set up a figure that can track the Wiimote's 3 acceleration
%            values

global wiimote;

% initialize the graph
wiimote.InitializeWmAccelGraph();

end