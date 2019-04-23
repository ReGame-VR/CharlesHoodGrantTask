function initializeWiimoteAccelGraph()
%  usage:    initializeWMAccelGraph()
%  purpose:  set up a figure that can track the Wiimote's 3 acceleration
%            values and plot them in real time

global wiimote;

% initialize the graph
wiimote.InitWiimoteAccelGraph();
