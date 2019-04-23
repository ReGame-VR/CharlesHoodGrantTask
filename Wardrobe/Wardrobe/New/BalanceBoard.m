classdef BalanceBoard < handle
    %BALANCEBOARD Balance Board Interface
    %   Handles communication with the wiifit and applies calibration
    %   to returned values
    
    properties %(SetAccess = private, GetAccess = private)
        wiifit
        gui
        calibration
        targetArea
        targetYellowArea
        targetThreshold = 3
        wiiPairPath = 'C:\Program Files (x86)\WiiPair\WiiPair.exe'
    end
    
    methods
        function obj = BalanceBoard(wiiPairPath)
            if nargin > 1
                obj.wiiPairPath = wiiPairPath;
            end
            obj.wiifit = Wiimote();
            obj.calibration = BalanceBoardCalibration();
            obj.setTargetArea([-5 5 -5 5]);
        end
        
        function delete(obj)
            obj.Disconnect();
        end
        
        function success = Connect(obj)
            if ~obj.isConnected()
                obj.wiifit.Connect();
            end
            success = obj.wiifit.isConnected() > 0;
        end
        
        function [status,result] = wiiPair(obj)
            [status,result] = system(obj.wiiPairPath);
        end
        
        function connected = isConnected(obj)
            connected = obj.wiifit.isConnected() > 0;
        end
        
        function Disconnect(obj)
            if obj.isConnected()
                obj.wiifit.Disconnect();
            end
        end
        
        function setCalibration(obj, newCalibration)
            obj.calibration = newCalibration;
        end
        
        function setTargetArea(obj, targetArea)
            obj.targetArea = targetArea;
            obj.targetYellowArea = [...
                obj.targetArea(1) - obj.targetThreshold ...
                obj.targetArea(2) + obj.targetThreshold ...
                obj.targetArea(3) - obj.targetThreshold ...
                obj.targetArea(4) + obj.targetThreshold ...
            ];
        end
        
        
        function [cog, sensorState, color] = getState(obj)
            % Get COG
            cog = obj.wiifit.wm.GetBalanceBoardCoGState();
            
            % Get raw sensor state and apply calibration
            rawSensorState = obj.wiifit.wm.GetBalanceBoardSensorState();
            sensorState = (rawSensorState - obj.calibration.offset) .* obj.calibration.scale;
            color = obj.getTargetColor(cog);
        end
    end
    
     methods (Access = 'private')
        
        function color = getTargetColor(obj, cog)
            %
            % Check if we're in the yellow range
            if cog(1) >= obj.targetYellowArea(1) ... 
                    && cog(1) <= obj.targetYellowArea(2) ...
                    && -cog(2) >= obj.targetYellowArea(3)  ...
                    && -cog(2) <= obj.targetYellowArea(4)
                
                % check if we're in the green range
                if cog(1) >= obj.targetArea(1) ...
                        && cog(1) <= obj.targetArea(2) ...
                        && -cog(2) >= obj.targetArea(3) ...
                        && -cog(2) <= obj.targetArea(4)
                    color = 1;
                else
                    color = 2;
                end
            else
                color = 3;
            end
            color = 1; 
        end
    end
end

