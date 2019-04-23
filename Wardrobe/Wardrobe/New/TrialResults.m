classdef TrialResults
    %TRIALRESULTS
    %   Results from run of a trial
    
    properties
        objectId
        targetId
        correctObject
        correctPosition
        timeToOpenDoor
        timeToPlaceObject
        score
        log 
    end
    
    methods
        function obj = TrialResults()
            obj.objectId = 0;
            obj.targetId = 0;
            obj.correctObject = 0;
            obj.correctPosition = 0;
            obj.timeToOpenDoor = 0;
            obj.timeToPlaceObject = 0;
            obj.score = 0;
            obj.log = {};
        end
    end
end

