 clear;

%% Set up Environment
setupPaths;

subjectId = input('Enter Subject ID:', 's');

%% Set Up App
% Create an instance of the wardrobe app and initialize it
app = Application();
app.initialize();

% Start running the trials
trialResults = app.runTrials();

app.close();

% Store results in matfile
filename = [subjectId '-' datestr(now, 'yyyymmddTHHMMSS') '-results'];
mf = matfile([filename '.mat']);
mf.trialResults = trialResults;

% Format as CSV
csvFile = fopen([filename '.csv'], 'w');

% Write header
fprintf(csvFile, 'Subject Id,Trial Number,Target #, Object #, Correct Object,Correct Position,Time To Open Door,Time To Place Object,Score\r\n');

% Write row per trial
[trialCount, ~] = size(trialResults);
for trialNo=1:trialCount
	result = trialResults{trialNo};
	fprintf(csvFile, '%s,%d,%d,%d,%d,%d,%1.3f,%1.3f,%1.3f\r\n', subjectId, trialNo, result.targetId, result.objectId, result.correctObject, result.correctPosition, result.timeToOpenDoor, result.timeToPlaceObject, result.score);
end
fclose(csvFile);

