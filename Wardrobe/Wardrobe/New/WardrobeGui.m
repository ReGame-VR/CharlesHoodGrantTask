classdef WardrobeGui < handle
    %UNTITLED Summary of this class goes here
    %   Detailed explanation goes here
    
    properties
        fig
        imageHandle
        textPanel
        
        trialNumberDisplay
        objectNumberDisplay
        timeDisplay
        scoreDisplay
    end
    
    methods
        function obj = WardrobeGui()
            mp = get(0, 'MonitorPositions')
            
            % If there are 2 monitors, show gui on second monitor
            % assumes monitor 2 is right of monitor 1
            
            if size(mp,1) == 1
               position = [0, 0, 1, 1];
            else
                position = [0, 1, 1, 1];
            end
            
            [obj.fig] = figure(...
                'Visible', 'off', ...
                'units', 'Normalized', ...
                'outerposition', position, ...
                'color', 'black', ...
                'Name', 'Wardrobe GUI'); 
            
            % Create Text Panel
            obj.textPanel = uipanel(...
                'Units', 'Normalized', ...
                'Position', [0.1, 0.75, 0.8, 0.2], ...
                'Parent', obj.fig);
            
            uicontrol(obj.textPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0, 0.6, 0.2, 0.40], ...
                'String', 'Trial: ', ...
                'fontsize', 50, ...
                'HorizontalAlignment', 'left');
            
            uicontrol(obj.textPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.5, 0.6, 0.2, 0.40], ...
                'String', 'Time: ', ...
                'fontsize', 50, ...
                'HorizontalAlignment', 'left');
            
            uicontrol(obj.textPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.5, 0.2, 0.2, 0.40], ...
                'String', 'Score: ', ...
                'fontsize', 50, ...
                'HorizontalAlignment', 'left');
            
            obj.trialNumberDisplay = uicontrol(obj.textPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.2, 0.6, 0.2, 0.40], ...
                'String', '', ...
                'fontsize', 50, ...
                'HorizontalAlignment', 'left');
            
            obj.timeDisplay = uicontrol(obj.textPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.7, 0.6, 0.2, 0.40], ...
                'String', '00:00.00', ...
                'fontsize', 50, ...
                'HorizontalAlignment', 'left');
            
            obj.scoreDisplay = uicontrol(obj.textPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.7, 0.2, 0.2, 0.40], ...
                'String', '0', ...
                'fontsize', 50, ...
                'HorizontalAlignment', 'left');
            
            % Create Image handle
            obj.imageHandle = axes(...
                'Units', 'pixels', ...
                'Position', [560, 100, 800, 600], ...
                'Parent', obj.fig);
            axis(obj.imageHandle, 'off');
            
            % Move to second monitor if applicable
            monitors = get(0, 'MonitorPositions');
            if size(monitors, 1) > 1
                set(obj.fig, 'Units', 'pixels');
                set(obj.fig, 'outerposition', monitors(2, 1:end));
                set(obj.fig, 'Units', 'Normalized');
            end
            
            set(obj.fig, 'Visible', 'on');
        end
        
        function delete(obj)
            if obj.fig.isvalid
                close(obj.fig);
            end
        end
        
        function setImage(obj, path)
            imageData = imread(path);
            image(imageData, 'Parent', obj.imageHandle);
            axis(obj.imageHandle, 'off');
        end
        
        function clearImage(obj)
            cla(obj.imageHandle);
            axis(obj.imageHandle, 'off');
        end
        
        function setTrialNumber(obj, trialNumber)
            set(obj.trialNumberDisplay, 'String', trialNumber);
        end
        
        function setObjectNumber(obj, objectNumber)
            set(obj.objectNumberDisplay, 'String', objectNumber);
            imageData = imread(obj.getImagePath(objectNumber));
            image(imageData, 'Parent', obj.imageHandle);
            axis(obj.imageHandle, 'off');
        end
        
        function setTime(obj, time)
            set(obj.timeDisplay, 'String', time);
        end
        
        function setScore(obj, score)
            set(obj.scoreDisplay, 'String', score);
        end
        
        function reset(obj)
            obj.setTime(0);
            obj.setScore(0);
            obj.setTrialNumber('');
            set(obj.objectNumberDisplay, 'String', '');
            obj.clearImage();
        end
        
        function close(obj)
            if obj.fig.isvalid
                close(obj.fig);
            end
        end
    end
    
    methods (Static)
        function path = getImagePath(index)
            path = sprintf('images/object%d.jpg', index);
        end
    end
    
end

