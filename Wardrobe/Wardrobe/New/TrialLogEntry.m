classdef TrialLogEntry
    %UNTITLED3 Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        time = 0
        cog = []
        sensorState = []
        doorState = []
        loadCellState = []
        color = 0
    end
    
    methods
        function obj = TrialLogEntry(time, cog, sensorState, doorState, loadCellState, color)
            if nargin > 0
                obj.time = time;
                obj.cog = cog;
                obj.sensorState = sensorState;
                obj.doorState = doorState;
                obj.loadCellState = loadCellState;
                obj.color = color;
            end
        end
    end
end

