% bbrecord is a script that runs a user friendly interface for viewing
% center of pressure data from a Wii Balance Board
%
% It uses the WiiLab patch that includes balance board support,
% http://klab.wikidot.com/wii-proj) 
% 
% System requirements: Windows XP & Matlab R2007a
%
% Authors of bbrecord: Members of the University of Colorado, Boulder 
% Neuromechanics Lab (PI Alaa Ahmed)

% versions 1_1-1_4: Andrew Kary
% version 1_0: Helen J. Huang, Sergio Perez

% this version fixes an error that existed in version 1_3 in which
% sequential trials could not be recorded using timed recording.

% Future ideas:
% fix the mysterious issue, don't use the run twice hack
% clean up the code, use data structures
% add functionality for opening and plotting old trial data using winopen

%% Setting the path

global bb f go flag_saving connected quitgui collecting filename ...
    deletedfile enable_results reset C sensor_avg_upright saveduration ...
    h_timerinput h_startbutton h_timerbutton flag_stop flag_timed...
    flag_start starttoggle Upper_time drivename h_resultsbutton;

% housekeeping
if ~exist('connected','var') || isempty(connected)
    clear all;
    delete first.mat;
end
close all; clc; clear first

% find the path for this m file
thisone = mfilename('fullpath');
backslashes = find(thisone == '\');
thispath = thisone(1:(backslashes(end)-1));
cd (thispath) % make sure that we are in the directory of this file
% check if this is the first time that bbrecord has been run 
if ~exist('first.mat','file') || isempty(drivename)
    first = 1;    
else
    load first;
end

if first == 1
    
    % find the address of the directory
    [stat,drive] = fileattrib;

    % find all of the slashes
    slashes = find('\' == drive.Name);

    % find the address of the folder that the directory is nested inside of
    drivename = drive.Name( 1 : slashes(end)-1 );

    % add the path of the home folder, CU_Wii
    addpath([drivename '\CU_Wii'])
    
    % add the path containing the Wii balance board function we wrote
    addpath([drivename '\CU_Wii\WiiBBFunctions'])

    % add the path containing simple graphics functions of WiiLAB
    addpath([drivename '\WiiLAB\WiiLAB_Matlab\EG111-H'])

    % add the path containing the Wiimote functions of WiiLAB
    addpath([drivename '\WiiLAB\WiiLAB_Matlab\WiimoteFunctions'])

    % create sound player
    [Y,FS] = audioread('C:\Users\RegameVRLab\Desktop\WiiProject\CU_Wii\WiiBBFunctions\bloop.au');
    
    player = audioplayer(Y, FS);
    
    save first first
end

%% Make sure that MATLAB talks to the Balance Board

if ~exist('connected','var') || (isempty(connected)) || (connected == 0)

    % from WiiLAB wiimote.m
    bb = Wiimote();
    bb.Connect(); % connect to balance board

    connected = 1;

end

if first == 0
    % set path for saving data
    pathname = [drivename '\CU_Wii\Data'];
    %pathname = [drivename '\CU_Wii'];

    % change to that directory
    cd(pathname);

%     answer = questdlg('Would you like to open the data folder?', ...
%         'View Data Folder?', 'Yes', 'No','Yes');
%     switch answer,
%         case 'Yes',
%             winopen(pathname);
%         case 'No',
%             cal = 0;
%     end


end

% subID = 'xxx';

%% Balance Board Data

% check that the balance board is REALLY connected
if bb.isConnected() > 0

    %     bb.wm.GetBalanceBoardCoGState(); % returns rough values for the center-of-pressure into variable called 'cog' x,y [cm]. positive direction is right, back
    %     bb.wm.GetBalanceBoardSensorState(); % returns values for the 4 sensors into a variable called 'sensors' [no obvious units]
    %     bb.wm.GetBatteryState(); % 1 for full charge, 0 for absolutely empty

if ~exist('XMaxWeight','var') || ~exist('XMinWeight','var') || ~exist('YMaxWeight','var') || ~exist('zeroX','var') || ~exist('YMinWeight','var') || ~exist('zeroY','var')
    XMaxWeight = 14;
    XMinWeight = -14;
    YMaxWeight = 10;
    YMinWeight = -10;
    zeroX = 0;
    zeroY = 0;
end

%% Calibrate
    if first == 0
        
        C = [5, 5, 5, 5];
        sensor_avg_upright = [-4.9, -3.7, -0.35, -0.89];
        
        if(~exist('subID'))
           subID = 'default_';
           disp('subID given default value');
        end
 C(1) == 0
            cal = 1; % do this if the BB has not been calibrated
        if ~exist('sensor_avg_upright','var') || isempty(sensor_avg_upright)
        else
            % do this if the BB has been calibrated
            answer = questdlg('Would you like to change the subject ID?', ...
                'Calibrate?', 'Yes', 'No','Yes');
            switch answer,
                case 'Yes',
                    cal = 1;
                case 'No',
                    cal = 0;
            end
        end

        if cal == 1
            prompt = 'Enter the ID of the subject:\n';
            subID = input(prompt,'s');          
            %[C, quitgui, sensor_avg_upright ] = OneWeightCalibrate (bb); % calibrate
          %  [zeroX, zeroY, XMaxWeight, XMinWeight, YMaxWeight, YMinWeight] = getMaxWeight(bb); %weight shift bounds calibration
%             [zeroX, zeroY, XMaxWeight, XMinWeight, YMaxWeight, YMinWeight] = getMaxWeight(bb); %weight shift bounds calibration
            answer = questdlg('Subject taking trial is short or tall?', ...
                             'View Data Folder?', 'Short', 'Tall','Yes');
            switch answer,
                case 'Short',
                    XMaxWeight = 12;
                    XMinWeight = -12;
                    YMaxWeight = 8;
                    YMinWeight = -8;    
                case 'Tall',
                    XMaxWeight = 14;
                    XMinWeight = -14;
                    YMaxWeight = 10;
                    YMinWeight = -10;;
            end

        end
        
            dt = datetime('now');
            DateString = datestr(dt);
            DateString(regexp(DateString,'[-, ,:]'))=[];
            dateToday = datestr(now,'mmmm dd, yyyy');
            
            dim = 6; %The width of the green target square
            halfdim = dim / 2;
            devdim = dim + 3; %The width of the yellow target square
            
            Atar = [XMinWeight/2, YMaxWeight - halfdim]; %Center coordinates of target A
            Btar = [XMaxWeight - halfdim, YMaxWeight - halfdim]; %Center coordinates of target B
            Ctar = [XMinWeight + halfdim, 0]; %Center coordinates of target C
            Dtar = [2 + halfdim, 0]; %Center coordinates of target D
            Etar = [0 - halfdim, YMinWeight + halfdim]; %Center coordinates of target E
            Ftar = [(XMaxWeight - 2) - halfdim, YMinWeight + halfdim]; %Center coordinates of target F

            VRString = strcat(subID, 'VRData', DateString, '.txt');
            CoPString = strcat(subID,'CoPData', DateString, '.txt');
            
            VRdata = fopen(VRString,'w'); %Create a text file to save the collected data to at the end of the trial
            fprintf(VRdata,'\t\t\tCalibration results:\n\n');
            fprintf(VRdata,'\t zeroX \t zeroY \t XMaxWeight \t XMinWeight \t YMaxWeight \t YMinWeight\n');
            formatSpec = '\t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \n';
            fprintf(VRdata, formatSpec, zeroX, zeroY, XMaxWeight, XMinWeight, YMaxWeight, YMinWeight);

            VRdata = fopen(VRString,'a'); %Create a text file to save the collected data to at the end of the trial
            fprintf(VRdata,'\n\n\t centerAx \t centerAy \t centerBx \t centerBy \t centerCx \t centerCy \t centerDx \t centerDy \t centerEx \t centerEy \t centerFx \t centerFy\n');
            formatSpec = '\t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \t %3.2f \n';
            fprintf(VRdata, formatSpec, Atar(1), Atar(2), Btar(1), Btar(2), Ctar(1), Ctar(2), Dtar(1), Dtar(2), Etar(1), Etar(2), Ftar(1), Ftar(2));
            
            VRdata = fopen(VRString,'a'); %Create a text file to save the collected data to at the end of the trial
            fprintf(VRdata, '\n\n\t trialNum \t date \t time \t targetlNum \t targetTime \t Weightshift success \t button success \t random or set sequence \t X coord of target \t Y coord of target \t COPtotalpath \t ScorePerTarget \t ScorePerTrial \t CumulativeScore \n\n');
            fclose(VRdata);

            CoPdata = fopen(CoPString,'w'); %Create a text file to save the collected center of pressure data to at the end of the trial
            fprintf(CoPdata, '\t time \t\t X \t\t Y\n\n');
            fclose(CoPdata); 

    end
%End of balance board setup

%% !! PARAMETERS TO SET !!
dim = 6; %The width of the green target square
halfdim = dim / 2;
devdim = dim + 3; %The width of the yellow target square

Atar = [XMinWeight/2, YMaxWeight - halfdim]; %Center coordinates of target A
Btar = [XMaxWeight - halfdim, YMaxWeight - halfdim]; %Center coordinates of target B
Ctar = [XMinWeight + halfdim, 0]; %Center coordinates of target C
Dtar = [2 + halfdim, 0]; %Center coordinates of target D
Etar = [0 - halfdim, YMinWeight + halfdim]; %Center coordinates of target E
Ftar = [(XMaxWeight - 2) - halfdim, YMinWeight + halfdim]; %Center coordinates of target F
targets = [Etar; Ctar; Atar; Btar; Dtar; Ftar]; %All of the targets combined into one 2D array, in the correct order

score = 0;
cum_score = 0;
trial_score = 0;
timeout = 0;
time = 0;
valid = 0;



[y,correct] = audioread('beep-02.wav'); %Read in the audio file for the correct sound effect 
[t,wrong] = audioread('wrong.wav'); %Read in the audio file for the wrong sound effect

comPortL = 'COM5'; %Set the com port the arduino is connected to here. To find that type instrfind
comPortB = 'COM4'; %This is the arduino that controls the buttons   

setSeq = [4, 1, 3, 5, 2]; %Set the predefined sequence here
length = numel(setSeq);

testTime = 10; %Set the amount of time you want for the subject to touch the button, in seconds
trialNum = 41; %Set the number of trials you would like to do in a row
waitTime = 0; %Set the amount of time the subject needs to be correctly weight shifted before the trial is counted as a success

button = 0;
count = 1; %Position in data array
datacount = 1; %Position in center of pressure data array
side = 0; %Variable to determine which weight shift number is needed. 1 is left, 2 is right
dataToWrite = [0, 0, 0, 0, 0, 0, 0]; %Initialize the data array with 0s
%The 6 values are cputime, trial num, trial time, weight shift success, button success,
%and random or set sequence

rng('shuffle')

ardButton = arduino(comPortB, 'uno');

configurePin(ardButton, 'D2', 'pullup'); %Configure all the button pins
configurePin(ardButton, 'D3', 'pullup');
configurePin(ardButton, 'D4', 'pullup');
configurePin(ardButton, 'D5', 'pullup');
configurePin(ardButton, 'D6', 'pullup');

ardLight = setupSerial(comPortL); %Let setupSerial set up the serial connection at the defined port

%arduino send code to matlab
s=serial(comPortL,'BaudRate',19200);
fopen(s);
s.InputBufferSize = 8;



beep on;
%% Create GUI figure for displaying BB data

if first == 0
    
    saveduration = 10; % How long do you want to save the data for by default?
    cycletime = 1/30 ; % how long should each iteration take? [seconds]
    Upper_time = 3600; % longest allowable trial [s]
    
    screensize = get(0,'ScreenSize');
    % figure('units','normalized','outerposition',[0 0 1 1])
%     scorefig = figure('Position', [1902 -230 988.3 879], 'Name', 'Score Window');
    mntrs = get(0, 'MonitorPositions');
    scorefig = figure('Position', [mntrs(2,1) mntrs(2,2) mntrs(2,3) mntrs(2,4)], 'Name', 'Score Window');
    axis off;
    set(gcf,'color','white')
    
    %     text(.7, .1, 'Current Score: ', 'fontsize', 13);
%     scoreForm = text(0.85, .1, '0', 'fontsize', 13);
%     scorefig = figure('Position', [0 0 500 500], 'Name', 'Score Window');
    text(.1, .7, 'trial score: ', 'fontsize', 26);
    scoreForm = text(0.5, .7, '0', 'fontsize', 26);
    text(.1, .6, 'total score: ', 'fontsize', 26);
    cumScore = text(0.5, .6, '0', 'fontsize', 26);
    msg = text(.1, .5, ' ', 'fontsize', 26);
    set(scorefig,'Visible','on'); % show the figure
    
    f = figure('Visible','off','color',[0.8 0.8 0.8],'units','normalized','outerposition',[0 0 1 1],'Name','Wii Balance Board GUI');
%     f = figure('Visible','off','color',[0.8 0.8 0.8],'units','normalized','Position', [0 0 mntrs(1,3) mntrs(1,4)],'Name','Wii Balance Board GUI');
    % maxfig(f,1);

    subplot('position', [.1  .8  .8  .2]);
    set(gca, 'visible','off','Units', 'normalized');

    x1 = -.1;
    x2 = .05;
    x3 = .15;
    x4 = .26;
    x5 = .7;
    x6 = 0.5;
    
    y0 = .7;
    y1 = .6;
    y2 = .5;
    y3 = .3;
    y4 = .2;
    y5 = .1;

    % COP Text
    x_copDispLbl = text(x1,0.9,'CoP X [cm] = ','fontsize', 13);
    y_copDispLbl = text(x1,0.8,'CoP Y [cm] = ','fontsize', 13);
    x_copDisp = text(x2, 0.9, 'xx','fontsize', 13);
    y_copDisp = text(x2, 0.8, 'xx','fontsize', 13);

    % Force sensor values
    bb_BLDispLbl = text(x1, 0.6, 'Bottom Left force [kgf] = ','fontsize', 13);
    bb_BRDispLbl = text(x1, 0.5, 'Bottom Right force [kgf] = ','fontsize', 13);
    bb_TLDispLbl = text(x1, 0.4, 'Top Left force [kgf] = ','fontsize', 13);
    bb_TRDispLbl = text(x1, 0.3, 'Top Right force [kgf] = ','fontsize', 13);
    bb_TotalispLbl = text(x1, 0.15, 'Total force [kgf] = ','fontsize', 13);

    bb_BLDisp = text(x3, 0.6, 'xx','fontsize', 13);
    bb_BRDisp = text(x3, 0.5, 'xx','fontsize', 13);
    bb_TLDisp = text(x3, 0.4, 'xx','fontsize', 13);
    bb_TRDisp = text(x3, 0.3, 'xx','fontsize', 13);
    bb_Totaldisp = text(x3, 0.15, 'xx','fontsize', 13);

    % Text indicating some Parameters
    text(x5, y1, 'Number of trials:', 'fontsize', 13);
    text(x5, y3, 'Current sequence: ', 'fontsize', 13);
    text(x5, y2, 'Light Number: ', 'fontsize', 13);
    text(x5, y4, 'Elapsed Time: ', 'fontsize', 13);
 
    xrange = text(0.85, y1, 'xx', 'fontsize', 13);
    xmean = text(0.85, y2, 'xx', 'fontsize', 13);

    yrange = text(0.85, y3, 'xx', 'fontsize', 13);
    ymean = text(0.85, y4, 'xx', 'fontsize', 13);
    
    for i=1:6
        point(1) = targets(i, 1) - halfdim;
        point(2) = targets(i, 1) + halfdim;
        point(3) = targets(i, 2) - halfdim;
        point(4) = targets(i, 2) + halfdim;
                        
        % points of the target square.
        X=[point(1), point(2)];
        Y=[point(3), point(3)];
        X1=[point(2), point(2)];
        Y1=[point(3), point(4)];
        X2=[point(1), point(1)];
        Y2=[point(4), point(4)];
        
        subplot('position',[.1  .1  .8  .7]);
        axis([-22.5 22.5 -13 13]);
        line(X,Y);
%         line(X2_cor,Y1_cor,'Color','r');
        line(X1,Y1);
        line(X,Y2);
        line(X2,Y1);
%         line(X_cor,Y_cor,'Color','r');
%         line(X1_cor,Y1_cor,'Color','r');
%         line(X_cor,Y2_cor,'Color','r');
        xlabel('X [cm]');
        ylabel('Y [cm]');
        set(gca, 'fontsize', 13); grid on;
%         legend({'Original targets', 'Targets after calibration'},'FontSize',8,'FontWeight','bold')
        
        
    end
    % Text indicating SCORE Parameters
%     text(.2, .1, 'Score on trial:', 'fontsize', 13);
%     text(.2, .3, 'Cumulative score: ', 'fontsize', 13);

    % Initialize variables
    reset = 1;
       
end

%% Get BB Data
    if first == 0
        
        % initialize
        quitgui = 0; % 1 for quit

        while quitgui == 0

            % get rid of old data
            if reset == 1
                
               % set(xrange, 'String', TrialNum, 'Color', 'k' );
                
               %  set(xmean, 'String', 'xx', 'Color', 'k' );
               %  set(yrange, 'String', 'xx', 'Color', 'k' );
               %  set(ymean, 'String', 'xx', 'Color', 'k' );
               %  set(h_startbutton, 'Enable', 'on')
               %  set(h_resetbutton, 'Enable', 'off')
               %  set(save_duration_text, 'String', 'xx', 'color', 'k');
               %  set(savestatustext, 'Color', 'k', 'String', 'Recording OFF');
               %  set(savenametext,'visible','off');
                
                clear first
                if exist('first','file')
                    delete first.mat
                end

                reset = 0; % don't need to reset anymore
                timestartsaving = 0; currentsaveduration = 0;
                iter = 0; % loop iteration number
                go = 1; % set go = 1 to execute loop, after reaching saveduration, go set to 0, stopping the loop
                collecting =1;
                trials = 0; % number of trials so far
                color = 'k';
                deletedfile = 0;
                enable_results = 0;
                starttoggle = 1;
                clear answer
                datalog = zeros(2*Upper_time/cycletime,8);
                fclose all;

                if flag_timed == 1
                    set(h_timerinput,'Enable','on');
                end
                
%                 formatSpecScore = {'Your score is: %4.2f.' 'Your Cumulative score is: %4.2f'};
%                 str = sprintf(formatSpecScore,score, cum_score);
%                 ScoreTrialBox = msgbox({'Your score is: 0.00.' 'Your Cumulative score is: 0.00'});
%                 set(ScoreTrialBox,'color',.85*[1 1 0]);

                set(f,'Visible','on'); % show the figure
                tStart = tic; % reset stopwatch
                
            end

            while go == 1
                
                for j = 1:trialNum
                                    
%                     trial_score = 0;
                    figure(scorefig);
                    set(scoreForm, 'String', trial_score);
                    set(cumScore, 'String', cum_score);
                    
                    if j>1
                        %text(.1, .5, 'Good job! Try to beat this score!', 'fontsize', 26);
                        set(msg,'String', 'Good job! Try to beat this score!');
                    end
                    
                    trialBox = msgbox('Press the SPACEBAR to start the next trial or press the Q key to quit');
                    movegui(trialBox, 'north');
                    
                    currkey = getkey;
                    while currkey ~= 32 && currkey ~= 113 %Get stuck in this while loop until the user presses key 32 (SPACEBAR) or key 113 (Q)
                        currkey = getkey;
                    end
                    delete(trialBox); %Get rid of the message box after the user presses the SPACEBAR or Q
                    
                    if currkey == 113 %Check for a press of key 113 (Q)
                        go = 2
                        quitgui = 1
                        break
                    end
                    
                    
                    set(msg, 'String', ' ');
                    trial_score = 0;
                    set(scoreForm, 'String', trial_score);
                    
                    decide = randi([0,1]) %Randomly choose random or set sequence
 %                  decide = 0;
    
                    for i = 1:length
                        figure(f); %Reset the figure
                        set(0,'CurrentFigure',f)
                        button = 0;
                        wghtScs = 0;
                        btnScs = 0;
                        timeout = 0;
                        time = 0;
                        valid = 0;
                        targetTime = 10.00;
                        COP_length = 0;
                        COP_distance = 0;
                        p1 = zeros(1,2);
                        p2 = zeros(1,2); %initialize the coordinates of two points to be both (0,0)
                        
                        if decide == 0 %Set sequence
                            lightNum = setSeq(i);
                        elseif decide == 1 
                              lightNum = randi([1,6]);
                           % lightNum = setSeq(i);  % in both cases set repeated sequence (exlude random sequence)
                            if i == 1
                                pastLightNum(i) = lightNum;
                            else 
                                while lightNum == pastLightNum(i-1)
                                    lightNum = randi([1,6]);
                                end
                                pastLightNum(i) = lightNum;
                            end
                        end
                        
                        %target X,Y coordinates: x1,x2,y1,y2
                        point(1) = targets(lightNum, 1) - halfdim;
                        point(2) = targets(lightNum, 1) + halfdim;
                        point(3) = targets(lightNum, 2) - halfdim;
                        point(4) = targets(lightNum, 2) + halfdim;
                        
                        % points of the target square.
                        X=[point(1), point(2)];
                        Y=[point(3), point(3)];
                        X1=[point(2), point(2)];
                        Y1=[point(3), point(4)];
                        X2=[point(1), point(1)];
                        Y2=[point(4), point(4)];
                        
                        %devtarget X,Y coordinates: x1,x2,y1,y2
                        devtarget(1) = targets(lightNum, 1) - devdim;
                        devtarget(2) = targets(lightNum, 1) + devdim;
                        devtarget(3) = targets(lightNum, 2) - devdim;
                        devtarget(4) = targets(lightNum, 2) + devdim;
                        
                        % points of the target square.
                        devX=[devtarget(1), devtarget(2)];
                        devY=[devtarget(3), devtarget(3)];
                        devX1=[devtarget(2), devtarget(2)];
                        devY1=[devtarget(3), devtarget(4)];
                        devX2=[devtarget(1), devtarget(1)];
                        devY2=[devtarget(4), devtarget(4)];
               
                        tic
                        newTrial = true;
                        togglePoint = 1;
                        while (toc < testTime)
                                                            
                            iter = iter + 1; %increment iteration counter

                            %data.bb.time(iter) = toc(tStart); % find the elapsed time now
                            cog = bb.wm.GetBalanceBoardCoGState(); % find the center of pressure
                            CoPDataArr(datacount, :) = [cputime, cog(1), -cog(2)];
                            datacount = datacount + 1;
                            sensors = (bb.wm.GetBalanceBoardSensorState()-sensor_avg_upright)./C; % find the force on each sensor, calibrated
                            
                            if (newTrial)
                                p1(1) = cog(1);
                                p1(2) = -cog(2);
                                newTrial = false;
%                                 disp(p1);
%                                 disp(p2);
                            else
                                if(togglePoint)
                                   p2(1) = cog(1);
                                   p2(2) = -cog(2);
                                else
                                   p1(1) = cog(1);
                                   p1(2) = -cog(2);
                                end
%                                 disp(p1);
%                                 disp(p2);
                                togglePoint = 1 - togglePoint; % toggle the point the current COP coordinates are written to.
                                COP_distance = sqrt((abs(p1(1)-p2(1)))^2 + (abs(p1(2)-p2(2)))^2); % calculate the distance between the current consequtive COP point
                                COP_length = COP_length + COP_distance;
                            end

                            %rectangle('Position', [XMinWeight, YMinWeight, (XMaxWeight - XMinWeight), (YMaxWeight - YMinWeight)]);

                            switch lightNum
                                case 1
                                    %button = readDigitalPin(ardButton, 'D2');
                                    readData=fscanf(s);
%                                     while(readData(1) ~= '\n')
%                                         readData = fscanf(s);
%                                     end
                                    button = str2double(readDat(2));
                                case 2
                                    %button = readDigitalPin(ardButton, 'D3');
                                    readData=fscanf(s);
                                    while(readData(1) ~= 'B')
                                        readData = fscanf(s);
                                    end
                                    button = str2double(readData(3));
                                case 3
                                    %button = readDigitalPin(ardButton, 'D4');
                                    readData=fscanf(s);
                                    button = str2double(readData(4));
                                case 4
                                    %button = readDigitalPin(ardButton, 'D5');
                                    readData=fscanf(s);
                                    while(readData(1) ~= 'B')
                                        readData = fscanf(s);
                                    end
                                    button = str2double(readData(5));
                                case 5
                                    %button = readDigitalPin(ardButton, 'D6');
                                    readData=fscanf(s);
                                    while(readData(1) ~= 'B')
                                        readData = fscanf(s);
                                    end
                                    button = str2double(readData(6));
                                case 6
                                    %button = readDigitalPin(ardButton, 'D7');
                                    readData=fscanf(s);
                                    while(readData(1) ~= 'B')
                                        readData = fscanf(s);
                                    end
                                    button = str2double(readData(7));
                            end %End of switch
                             pause(.01);


                            % fix iteration duration
                            t_el = toc(tStart);
                            if t_el/iter < cycletime
                                 pausetime = (cycletime*iter) - t_el;
                                 pause (pausetime)
                            end

                            % change text on GUI
                            set(xrange, 'String', trialNum);
                            set(xmean, 'String', lightNum);
                            set(yrange, 'String', i);
                            set(ymean, 'String', toc);

                            set(x_copDisp, 'String', cog(1));
                            set(y_copDisp, 'String', -cog(2));
                            set(bb_BLDisp, 'String', sensors(1));
                            set(bb_BRDisp, 'String', sensors(2));
                            set(bb_TLDisp, 'String', sensors(3));
                            set(bb_TRDisp, 'String', sensors(4));
                            weight = sum(sensors);
                            set(bb_Totaldisp, 'String', weight);


                            % plot the CoP
                            size = weight/1.5;
                            if size < 1
                                size = 1;
                            end

                            if devtarget(1)<=cog(1)&& cog(1)<=devtarget(2) && devtarget(3)<=-cog(2)&& -cog(2)<=devtarget(4) % COP is inside yellow area
                                wghtScs = 1;
                                if point(1)<=cog(1)&& cog(1)<=point(2) && point(3)<=-cog(2)&& -cog(2)<=point(4) % COP is inside the targeted area
                                    greenStart = tic; %Reset stopwatch
                                    while point(1)<=cog(1)&& cog(1)<=point(2) && point(3)<=-cog(2)&& -cog(2)<=point(4) && toc < testTime
                                        figure(f); %Reset the figure

                                        % change text on GUI
                                        set(xrange, 'String', trialNum);
                                        set(xmean, 'String', lightNum);
                                        set(yrange, 'String', i);
                                        set(ymean, 'String', toc);
                                        set(x_copDisp, 'String', cog(1));
                                        set(y_copDisp, 'String', -cog(2));
                                        set(bb_BLDisp, 'String', sensors(1));
                                        set(bb_BRDisp, 'String', sensors(2));
                                        set(bb_TLDisp, 'String', sensors(3));
                                        set(bb_TRDisp, 'String', sensors(4));
                                        weight = sum(sensors);
                                        set(bb_Totaldisp, 'String', weight);

                                        % plot the CoP
                                        size = weight/1.5;
                                        if size < 1
                                            size = 1;
                                        end                           
                                        pause(.01);

                                        subplot('position',[.1  .1  .8  .7]);
                                        plot(cog(1), -cog(2),'go', 'MarkerFaceColor','g', 'MarkerSize', size);
                                        %line(X1,Y1);
                                        %line(X,Y2);
                                        %line(X2,Y1);
                                        axis([-22.5 22.5 -13 13]);
                                        xlabel('X [cm]')
                                        ylabel('Y [cm]')
                                        set(gca, 'fontsize', 13); grid on;
                                        fprintf(ardLight,'%f',(1000*lightNum)+100); %Send the GREEN light number to the arduino
                                        
                                        cog = bb.wm.GetBalanceBoardCoGState(); % find the center of pressure
                                        CoPDataArr(datacount, :) = [cputime, cog(1), -cog(2)];
                                        datacount = datacount + 1;
%                                         sensors = (bb.wm.GetBalanceBoardSensorState()-sensor_avg_upright)./C; % find the force on each sensor, calibrated

                                if(togglePoint)
                                   p2(1) = cog(1);
                                   p2(2) = -cog(2);
                                else
                                   p1(1) = cog(1);
                                   p1(2) = -cog(2);
                                end
%                                 disp(p1);
%                                 disp(p2);
                                togglePoint = 1 - togglePoint; % toggle the point the current COP coordinates are written to.
                                COP_distance = sqrt((abs(p1(1)-p2(1)))^2 + (abs(p1(2)-p2(2)))^2); % calculate the distance between the current consequtive COP point
                                COP_length = COP_length + COP_distance; % update total COP length
                                        
                                        switch lightNum
                                            case 1 %slave PRESS#
                                                %button = readDigitalPin(ardButton, 'D2');
                                                readData=fscanf(s);
                                                while(readData(1) ~= 'B')
                                                    readData = fWiiBalBrd2Aug03_noCalibShortTall.m
                                                %button = readDigitalPin(ardButton, 'D3');
                                                readData=fscanf(s);
                                                end
                                                while(readData(1) ~= 'B')
                                                    readData = fscanf(s);
                                                end
                                                button = str2double(readData(3));
                                            case 3
                                                %button = readDigitalPin(ardButton, 'D4');
                                                readData=fscanf(s);
                                                button = str2double(readData(4));
                                            case 4
                                                %button = readDigitalPin(ardButton, 'D5');
                                                readData=fscanf(s);
                                                while(readData(1) ~= 'B')
                                                    readData = fscanf(s);
                                                end
                                                button = str2double(readData(5));
                                            case 5
                                                %button = readDigitalPin(ardButton, 'D6');
                                                readData=fscanf(s);
                                                while(readData(1) ~= 'B')
                                                    readData = fscanf(s);
                                                end
                                                button = str2double(readData(6));
                                            case 6
                                                %button = readDigitalPin(ardButton, 'D7');
                                                readData=fscanf(s);
                                                while(readData(1) ~= 'B')
                                                    readData = fscanf(s);
                                                end
                                                button = str2double(readData(7));
                                        end %End of switch
              
                                        if toc(greenStart) > 1
                                            timeout = 1;
                                            if button == 1
                                                btnScs = 1;
                                                %score = score + 1;
                                                break
                                            end
                                            
                                        end
                  
                                    end
                                    
                                    if timeout == 1 && btnScs == 1
                                        time = toc;
                                        if (toc > 10.00)
                                            targetTime = 10.00;
                                        else
                                            targetTime = toc;
                                        end
%                                         score = (time-1)+score;
%                                         time = round(time*100)/100; % round the number to have just two decimal points
                                        timeNow = datestr(now,'HH:MM:SS.FFF AM');
                                        %score = 10 - ceil((time-1));
                                        readData=fscanf(s);
                                        score = str2double(readData(7))*10 + str2double(readData(8)); 
                                        trial_score = trial_score + score;
                                        cum_score = cum_score+score;
                                        figure(scorefig);
                                        set(scoreForm, 'String', trial_score);
                                        set(cumScore, 'String', cum_score);
                                        valid = 1;
                                        break %Break out of the main while loop if the button is pressed after 1 second in the green region
                                    end
                                    
                                    
                                    


                                else % COP is outside the target area
                                    if button == 1
                                        btnScs = 1;
                                    else
                                        btnScs = 0;
                                    end
                                    figure(f); %Reset the figure
                                    subplot('position',[.1  .1  .8  .7]);
                                    plot(cog(1), -cog(2),'yo', 'MarkerFaceColor','y', 'MarkerSize', size);
                                    axis([-22.5 22.5 -13 13]);
                                    line(X,Y);
                                    line(X1,Y1);
                                    line(X,Y2);
                                    line(X2,Y1);
                                    xlabel('X [cm]')
                                    ylabel('Y [cm]')
                                    set(gca, 'fontsize', 13); grid on;
                                    fprintf(ardLight,'%f',(1000*lightNum)+ 1); %Send the YELLOW light number to the arduino
                                end

                            else % not enough weightshift
                                wghtScs = 0;
                                if button == 1
                                    btnScs = 1;
                                else
                                    btnScs = 0;
                                end
                                figure(f); %Reset the figure
                                subplot('position',[.1  .1  .8  .7]);
                                plot(cog(1), -cog(2),'ro', 'MarkerFaceColor','r', 'MarkerSize', size);
                                axis([-22.5 22.5 -13 13]);
                                xlabel('X [cm]')
                                ylabel('Y [cm]')
                                set(gca, 'fontsize', 13); grid on;
                                fprintf(ardLight,'%f',(1000*lightNum)+10); %Send the RED light number to the arduino

                            end

                        end % End of toc while loop
                        
                        % Either time elapsed or target found
                        %Change the score on the GUI
                        
                        if valid == 0
                            sound(t, wrong); %Play the "incorrect" sound
                            score = 0;
                        elseif valid == 1
                            sound(y, correct); %Play the "correct" sound
                        end
%                         figure(scorefig);
%                         scoreBox = uicontrol('style','text')
%                         set(scoreBox,'String',score)

                        VRdata = fopen(VRString,'a');
                        formatSpec = '\t %d \t %s \t %s \t %d \t %3.2f \t %d \t %d \t %d \t %3.2f \t %3.2f \t %3.2f \t %d \t %d \t %d \n';
%                         fprintf(VRdata, formatSpec, cputime, (j-1)*length + i, toc, wghtScs, btnScs, decide, targets(lightNum, 1), targets(lightNum, 2));
%                         fprintf(VRdata, formatSpec, j, cputime, lightNum, toc, wghtScs, btnScs, decide, targets(lightNum, 1), targets(lightNum, 2));
%                           targetTime = 0;
%                          if (toc > 10.00)
%                              targetTime = 10.00;
%                          else
%                              targetTime = toc;
%                          end
                        fprintf(VRdata, formatSpec, j, dateToday, timeNow, lightNum, targetTime, wghtScs, btnScs, decide, cog(1), -cog(2), COP_length, score, trial_score, cum_score);
                        fclose(VRdata);
                        
                    end
                    beep
                    
                end
                figure(scorefig);
                %text(.1, .5, 'Good job! Try to beat this score!', 'fontsize', 26);
                set(msg,'String', 'Good job! Try to beat this score!');
                fprintf(ardLight,'%f',333); %Send the 333 to signalize THE END
                go=0;
                quitgui=1;
                clear ardButton;
                fclose(ardLight);
            end

            pause(.1) % this just makes debugging easier

        end
         clear ardButton;
         fclose(ardLight);
         
         CoPdata = fopen(CoPString,'a');
         CoPformatSpec = '\t%3.2f\t\t%3.2f\t\t%3.2f\t\t\n';
         for index = 1:datacount-1
            fprintf(CoPdata, CoPformatSpec, CoPDataArr(index, :));
         end
         fclose(CoPdata); %Close the file

    else
        first = 0;
        save first first
        
        clear ardButton; %Needed due to the "run twice hack"
        fclose(ardLight); %Needed due to the "run twice hack"
%         delete(VRString);
%         delete(CoPString);
        run (thisone) 
        delete first.mat;
        % this runs the script a second time. this is really a hack, but it
        % solves a mysterious problem. for an unknown reason, the script
        % would not work right until the second time it was run.
        % clear ardButton;
        fclose(ardLight);
        
    end

    clear ardButton;
    fclose(ardLight);
    fclose(s);
%     close all

else
    error('BB is not connected. Try restarting MATLAB')
end

% clear ardButton;
fclose(ardLight);
fclose(s);
% bb.Disconnect();