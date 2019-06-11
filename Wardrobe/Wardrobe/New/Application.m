classdef Application < handle
    properties
        
        % Main control GUI handle
        mainGui
        
        % Wardrobe GUI handle
        wardrobeGui
        
        % Wardrobe Control interface
        wardrobeController
        
        % Balance Board
        bb
        
        % Door associated with each target
        targetDoors = [1 1 1 2 2 2 3]
        
        % wav Data
        goodSound
        badSound
        
        %% Configuration Parameters%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
        
        % How often to update the GUI/application state
        updateRateHz = 10.0
        
        % Trial length in seconds
        trialLength = 15; 
        
        % Number of sequential trials to run
        numberOfTrials = 10;
        
        % How long to display the image for at the start of the trial
        imageDisplayTimeSeconds = 3
             
        targetCopAreas %%%will be defined in Application function based on handedness
        
        %%%Old COP areas
        
        %         targetCopAreas = { ...
        %             [-30 -10 0 15], ...
        %             [-30 -10 0 15], ...
        %             [-30 -10 0 15], ...
        %             [10 30 0 15], ...
        %             [10 30 0 15], ...
        %             [10 30 0 15], ...
        %             [-10 10 0 15]
        %         }
        
        %%% 5x5 target areas centered toward the middle back of the board
        %%% (bc easier to shift weight toward the heels without lifing feet
        %%% than the toes/balls of feet)
        %         targetCopAreas = { ...
        %             [-14 -9 0 5], ...
        %             [-14 -9 -5 0], ...
        %             [-14 -9 -10 -5], ...
        %             [9 14 0 5], ...
        %             [9 14 -5 0], ...
        %             [9 14 -10 -5] ...
        %             [-10 10 0 15] %%% 7
        %         }
        %
        
        %%% 11 x 11 cm
        %       targetCopAreas = { ...
        %             [-15 -4 -3 8], ...
        %             [-15 -4 -8 3], ...
        %             [-15 -4 -10 1], ...   %[-15 -4 -13 -2], ... % shifted up, bc not feasible to lean so far back while placing
        %             [4 15 -3 8], ...
        %             [4 15 -8 3], ...    %[4 15 -13 -2], ...
        %             [4 15 -10 1], ...
        %             [-5 16 -6 5] %%% 7
        %         }
        
        %%%
        
        % Load Cell calibration for each section.
        % each section is an array of 5 object load cell values.
        % i.e. 1, 1 is section 1 load cell weight of object 1. 2,3 is the
        % weight of object 3 on the load cell in section 2.
        % It's possible that these weights may be the same for all
        % sections.
        objectCalibration = [...
            [1000 2000 5000 7000 8000]
            [1000 2000 5000 7000 8000]
            [1000 2000 5000 7000 8000]
            [1000 2000 5000 7000 8000]
            [1000 2000 5000 7000 8000]
            [1000 2000 5000 7000 8000]
            [1000 2000 5000 7000 8000]
            ]
        
        % Minimum weight to determine that an object is present on the load
        % cell.
        objectPresentThreshold = 10;
        
        % How close the weight needs to be to the calibrated weight to
        % detect the object successfully.
        % objectWeightThreshold = 50000
        objectWeightThreshold = 10000;
        
        % %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
    end
    
    methods
        function obj = Application(hand)
            
            % COP target area for each target section
            % Xmin, Xmax, Ymin, Ymax
            
            % Targets:
            % 1: Upper Left, 2: Left Middle, 3: Lower Left, 4: Upper Right, 5:
            % Middle Right, 6: Lower Right, 7: Drawer
            
            if hand == 'R'
                %%%10 x 10 cm for RH dominant; 11 x 11 cm for LH
                obj.targetCopAreas = { ...
                    [-15 -4 -3 8], ...
                    [-15 -4 -8 3], ...
                    [-15 -4 -10 1], ...
                    [5 15 -2 8], ...
                    [5 15 -7 3], ...
                    [5 15 -9 1], ...
                    [-4 16 -5 5] %%% 7
                    };
            else
                %%%10 x 10 cm for LH dominant; 11 x 11 cm for RH
                obj.targetCopAreas = { ...
                    [-15 -5 -2 8], ...
                    [-15 -5 -7 3], ...
                    [-15 -5 -9 1], ...
                    [4 15 -3 8], ...
                    [4 15 -8 3], ...
                    [4 15 -10 1], ...
                    [-4 16 -5 5] %%% 7
                    }; 
            end
            
            obj.loadCalibration();
            
            obj.mainGui = MainGui();
            obj.wardrobeGui = WardrobeGui();
            obj.wardrobeController = WardrobeController('COM5', 19200);  %%%COM5 to COM4
            obj.bb = BalanceBoard();
            if ~obj.bb.isConnected()
                obj.bb.Connect();
            end
        end
        
        
        function delete(obj)
            obj.close();
        end
        
        function close(obj)
            obj.wardrobeController.close();
            obj.mainGui.close();
            obj.wardrobeGui.close();
            obj.bb.Disconnect();
        end
        
        function obj = initialize(obj)
            obj.wardrobeController.init();
            obj.wardrobeController.reset();
            
            % Load Audio Files
            obj.goodSound = audioread('sounds/good.wav');
            obj.badSound = audioread('sounds/bad.wav');
        end
        
        function reset(obj)
            obj.wardrobeController.reset();
            obj.wardrobeGui.reset();
            obj.mainGui.reset();
        end
        
        function tareScales(obj)
            obj.wardrobeController.tare();
        end
        
        
        function results = runTrials(obj)
            
            % Set GUI up in default state
            obj.wardrobeGui.reset();
            score = 0;
            results = cell(obj.numberOfTrials, 1);
            
            for trialNumber=1:obj.numberOfTrials
                % Select a target
                if trialNumber == 1
                    targetId = randi([1,7]);
                else % prevTargetId exists
                    %                     % to prevent 2 of the same targets in a row
                    while targetId == prevTargetId
                        targetId = randi([1,7]);
                    end
                end
                
                prevTargetId = targetId; % save targetId to compare for next round, preventing 2 in a row
                
                % Select an object - targets 5 and 7 sometimes has trouble with object 4!
                if targetId == 7
                    valid_objects = [1:3, 5];
                else
                    valid_objects = (1:5);
                end
                
                if trialNumber == 1
                    objectId = valid_objects(randi(length(valid_objects)));
                else % prevObjectId exists
                    % to prevent 2 of the same objects in a row
                    while objectId == prevObjectId
                        objectId = valid_objects(randi(length(valid_objects)));
                    end
                end
                
                disp(objectId); 
                
                prevObjectId = objectId; % save targetId to compare for next round, preventing 2 in a row
                
                % update GUI
                obj.mainGui.setTarget(targetId);
                obj.wardrobeGui.setTrialNumber(sprintf('%d / %d', trialNumber, obj.numberOfTrials));
                obj.wardrobeController.setColor(0);
                
                % Wait For Some input...
                obj.waitForUserInput('Get Ready', 'Click ok to start Trial.');
                
                % Run the trial and store the results
                trialResult = obj.runTrial(targetId, objectId);
                
                % Update total score
                score = score + trialResult.score;
                obj.wardrobeGui.setScore(score);
                
                % Store results
                results{trialNumber} = trialResult;
                
                %%% moved to section where sounds are played, to ensure message box displayed matches sound 
                %                 if trialResult.correctObject && trialResult.correctPosition
                %                     obj.waitForUserInput('Success!', 'Close all the doors and return the Object to the starting area!');
                %                 else
                %                     obj.waitForUserInput('Failure!', 'Click below!');
                %                 end
                
            end
            
            obj.mainGui.setTarget(0);
            obj.wardrobeGui.reset();
        end
        
        
        function results = runTrial(obj, targetId, objectId)
            
            tickCount = obj.trialLength * obj.updateRateHz;
            log = cell(tickCount, 1);
            targetDoor = obj.targetDoors(targetId);
            timeToOpenDoor = 0;
            timeToPlaceObject = 0;
            correctObject = false;
            score = 0;
            
            % Set Target ID in wardrobe controller
            obj.wardrobeController.setTarget(targetId);
            obj.wardrobeController.setTargetColor(targetId, 3);
            
            % Set Balance Board target area
            obj.bb.setTargetArea(obj.targetCopAreas{targetId});
            
            % Set Object Number in wardrobe GUI
            obj.wardrobeGui.setObjectNumber(objectId);
            trialStartTime = tic;
            
            frameNumber = 0;
            
            while toc(trialStartTime) < obj.trialLength
                frameNumber = frameNumber + 1;
                frameStartTime = tic;
                elapsed = toc(trialStartTime);
                
                % Check for new Commands from main Gui
                % TODO...
                
                % Get Balance Board State
                [cog, sensorState, color] = obj.bb.getState();
                
                % Set LED color
                obj.wardrobeController.setTargetColor(targetId, color);
                
                % Get Wardrobe State
                [doorState, weightState] = obj.wardrobeController.readState();
                %disp(sprintf('Door: %d Weight: %1.2f', doorState(targetDoor), weightState(targetId)));
                
                log{frameNumber} = TrialLogEntry(elapsed, cog, sensorState, doorState, weightState, color);
                
                % Update GUIs
                obj.mainGui.setBalanceBoardState(cog, sensorState, color);
                obj.mainGui.setWardrobeState(doorState, weightState);
                
                if doorState(targetDoor) == 1 && timeToOpenDoor == 0
                    timeToOpenDoor = elapsed;
                end
                
                % Check for object
                if weightState(targetId) > obj.objectPresentThreshold
                    if abs(weightState(targetId) - obj.objectCalibration(targetId, objectId)) < obj.objectWeightThreshold
                        
                        % Got the right object!
                        timeToPlaceObject = elapsed;
                        correctObject = true;
                        
                        break
                    end
                end
                
                % Clear the image after some predetermined amount of time
                obj.wardrobeGui.setTime(elapsed);
                if elapsed > obj.imageDisplayTimeSeconds
                    obj.wardrobeGui.clearImage();
                end
                
                % Wait for frame time to elapse!
                frameEndTime = toc(frameStartTime);
                if frameEndTime < (1.0 / obj.updateRateHz)
                    pause((1.0/obj.updateRateHz) - frameEndTime);
                end
            end
            
            % Check to see that the last position was in the green area
            correctPosition = color == 1;
            
            if correctObject && correctPosition
                sound(obj.goodSound, 44100);
                obj.waitForUserInput('Success!', 'Close all the doors and return the Object to the starting area!');
                score = score + (obj.trialLength - timeToPlaceObject);
            else
                sound(obj.badSound, 44100);
                obj.waitForUserInput('Failure!', 'Click below!');
                % obj.wardrobeGui.setTime(obj.trialLength);
                score = 0;
            end
            
            obj.wardrobeGui.clearImage();
            obj.wardrobeController.setTarget(0);
            obj.wardrobeController.setColor(0);
            
            
            results.objectId = objectId;
            results.targetId = targetId;
            results.correctObject = correctObject;
            results.correctPosition = correctPosition;
            results.timeToOpenDoor = timeToOpenDoor;
            results.timeToPlaceObject = timeToPlaceObject; %about .28s of difference between time displayed on screen and time saved
            results.score = score;
            results.log = log;
        end
        
    end
    methods (Access = 'private')
        
        function loadCalibration(obj)
            if exist('calibration_data.mat', 'file')
                mf = matfile('calibration_data.mat');
                obj.objectCalibration = mf.objectCalibration;
            else
                disp('No calibration data found. Run Calibration script to calibrate load cells');
            end
        end
        
        function waitForUserInput(obj, name, text)
            d = dialog('Position',[300 300 250 150],'Name', name);
            
            uicontrol('Parent',d,...
                'Style','text',...
                'Position',[20 80 210 40],...
                'String', text);
            
            uicontrol('Parent',d,...
                'Position',[85 20 70 25],...
                'String','Ok',...
                'Callback','delete(gcf)');
            uiwait(d);
        end
    end
    
    
end




