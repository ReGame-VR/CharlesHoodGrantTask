close all;
clear;
mg = MainGui();
bb = BalanceBoard();
if ~bb.isConnected()
    bb.Connect();
end

for i=1:100
    [cog, ss, color] = bb.getState();
    mg.setBalanceBoardState(cog, ss, color);
    pause(1);
end

mg.close();
bb.Disconnect();
clear;
%app = Application();

%app.initialize();


%results = app.runTrials();
%app.close();