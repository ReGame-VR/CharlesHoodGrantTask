%% Set up path
% Restore default path
restoredefaultpath;
clear;


% find the address of the directory
[stat,drive] = fileattrib;
drivename = drive.Name;

% add the path containing the Wii balance board function we wrote
addpath([drivename '\WiiBBFunctions']);

% add the path containing simple graphics functions of WiiLAB
addpath([drivename '\WiiLAB\WiiLAB_Matlab\EG111-H']);

% add the path containing the Wiimote functions of WiiLAB
addpath([drivename '\WiiLAB\WiiLAB_Matlab\WiimoteFunctions']);
