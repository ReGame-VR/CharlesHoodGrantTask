instrreset;
%clear;
if exist('calibration_data.mat', 'file')
    mf = matfile('calibration_data.mat', 'Writable', true);
    cal = mf.objectCalibration;
else
    mf = matfile('calibration_data.mat', 'Writable', true);
    cal = zeros(7,5);
end

% Connect to wardrobe 
wardrobe = WardrobeController('COM5', 19200); %%%CHANGED COM5 TO COM4
wardrobe.init();

% Display message
scalesCleared = msgbox('Remove objects from all the scales, then hit SPACEBAR');
movegui(scalesCleared, 'north');

% Wait for spacebar press...
currkey = getkey;
while currkey ~= 32
    currkey = getkey;
end
delete(scalesCleared); %Get rid of the message box after the user presses the SPACEBAR



for target=1:7  %%%test: STARTING AT 2
    
    % Loop through targets and calibrate one at a time
    disp('Calibrating Target'); 
    disp(target);
    pause(0.5);
    % Turn on LED of current target.
    for i=1:10
        wardrobe.setTargetColor(target, 4);
        pause(0.1);
        wardrobe.setTarget(target);
    end
    
    
    % Get weight for each object
    for object=1:5

        % Display messagebox
        placeObject = msgbox(sprintf('Place object %d on scale then hit SPACEBAR', object));
        movegui(placeObject, 'north');
        
        % Wait for spacebar press...
        currkey = getkey;
        while currkey ~= 32
            currkey = getkey;
        end
        delete(placeObject);
        
        % Take 50 samples of the object's weight.
        weight_acc = 0;
        for i=1:50
            [doors, weights] = wardrobe.readState();
            disp(weights);
            weight_acc = weight_acc + weights(target);
            pause(0.1);
        end
        
        % Store the mean of the 50 samples to the calibration matrix.
        weight = weight_acc/50.0;
        cal(target,object) = weight;
        
        % If weight is 0 no data is coming back from the load cells.
        % Either not communicating with wardrobe or a bad load cell
        % connection.
        if weight == 0
            disp('No data returned from load cell, check connections and try again');
            wardrobe.close();
            return
        end
        disp('Calibrated weight:');
        disp(weight);
    end
end

% Close wardrobe connection.
wardrobe.close();

% Save calibration data
mf.objectCalibration = cal;