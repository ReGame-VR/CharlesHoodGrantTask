classdef WardrobeController < handle
    %WARDROBECONTROLLER Interface to wardrobe
    
    properties
        comPort = 'COM4'
        baudRate = 19200  
        serialPort
        
        % Target section
        % Target sections are:
        %
        % 0: No target section selected
        %
        % +-------+-----+-------+
        % |   1   |     |   4   |
        % +-------+-----+-------+
        % |   2   |     |   5   |
        % +-------+-----+-------+
        % |   3   |  7  |   6   |
        % +-------+-----+-------+
        targetSection = 0
        
        LED_COLOR_OFF = 0
        LED_COLOR_GREEN = 1
        LED_COLOR_YELLOW = 2
        LED_COLOR_RED = 3
        LED_COLOR_BLUE = 4
        LED_COLOR_WHITE = 5
        
        weights
        doors
        
        commandColors
        tareScales = 0
        
        messageTimestamp
        
        % Amount of elapsed time with no message that is considered an
        % error
        messageTimeoutSeconds = 0.5
       
        % Print debug messages?
        debug = 0
    end
    
    methods
        function obj = WardrobeController(comPort, baudRate)
            if nargin > 0
                obj.comPort = comPort;
                obj.baudRate = baudRate;
            end
            
            % Initialize serial port
            obj.serialPort = 0;
            
            obj.weights = zeros(1, 7);
            obj.doors = zeros(1, 3);
            obj.commandColors = zeros(1, 7);
        end
        
        function delete(obj)
            if obj.serialPort ~= 0;
                stopasync(obj.serialPort);
                fclose(obj.serialPort);
                delete(obj.serialPort);
                obj.serialPort = 0;
            end
        end
        
        function init(obj)
            obj.serialPort = serial(obj.comPort, 'BaudRate', obj.baudRate);
            obj.serialPort.InputBufferSize=128;
            obj.serialPort.Terminator = 'LF';
            flushinput(obj.serialPort);
            flushoutput(obj.serialPort);
            obj.serialPort.BytesAvailableFcn = @obj.dataCallback;
            fopen(obj.serialPort);
            readasync(obj.serialPort);
        end
        
        function connected = isConnected(obj)
            elapsed = toc(obj.messageTimestamp);
            connected = elapsed < obj.messageTimeoutSeconds;
        end
        
        function close(obj)
            % CLOSE  close the serial port
            stopasync(obj.serialPort);
            fclose(obj.serialPort);
            delete(obj.serialPort);
            obj.serialPort = 0;
        end
        
        function reset(obj)
            %RESET Reset the wardrobe controllers
            
            for i = 1:7
                obj.commandColors(i) = obj.LED_COLOR_OFF;
            end
            obj.targetSection = 0;
            obj.tareScales = 0;
            obj.updateWardrobe();
        end
        
        function setTarget(obj, target)
            obj.targetSection = target;
            obj.updateWardrobe();
        end
        
        function setColor(obj, color)
            %SETCOLOR Set LED colors for all LEDS
            % color - 1: green 2: yellow 3: red
            for i = 1:7
                obj.commandColors(i) = color;
            end
            obj.updateWardrobe();
        end
        
        function tare(obj)
            obj.tareScales = 1;
            obj.updateWardrobe();
            obj.tareScales = 0;
            obj.updateWardrobe();
        end
        
        function setTargetColor(obj, target, color)
            if target ~= 0
                for idx=1:7
                    if idx == target
                        obj.commandColors(idx) = color;
                    else
                        obj.commandColors(idx) = 0;
                    end
                end
            end
            obj.updateWardrobe();
        end
        
        
        function [doorState, weightState] = readState(obj)
            doorState = obj.doors;
            weightState = obj.weights;
        end
        
        function setState(obj, doors, weights)
            obj.doors = doors;
            obj.weights = weights;
        end
    end
    
    methods (Access = 'private')
        function updateWardrobe(obj)
            % Message format
            % Byte 1: sync byte [0x55]
            % Byte 2: target 1 color [0x00-0x03]
            % Byte 3: Target 2 color [0x00-0x03]
            % Byte 4: Target 3 color [0x00-0x03]
            % Byte 5: Target 4 color [0x00-0x03]
            % Byte 6: Target 5 color [0x00-0x03]
            % Byte 7: Target 6 color [0x00-0x03]
            % Byte 8: Target 7 color [0x00-0x03]
            % Byte 9: Scale Enabled  [0x00-0x07]
            % Byte 10: Tare [0x00-0x01]
            % Byte 11: <CR> [0x10]
            fwrite(obj.serialPort, 85, 'uint8');
            for i = 1:7
                fwrite(obj.serialPort, obj.commandColors(i), 'uint8');
            end
            fwrite(obj.serialPort, obj.targetSection, 'uint8');
            fwrite(obj.serialPort, obj.tareScales, 'uint8');
            fwrite(obj.serialPort, 10, 'char');
            pause(0.1);
        end

        function dataCallback(obj, src, evt)
            
            % Store the message received time.
            obj.messageTimestamp = tic;
            
            while obj.serialPort.BytesAvailable
                data = fgetl(obj.serialPort);
                if obj.debug
                    disp(['[WardrobeController] ' data]);
                end
                fields = strsplit(data, ',');
                for idx=1:length(fields)
                   parts = strsplit(fields{idx}, ':');
                   if length(parts) == 2
                       key = parts{1};
                       if numel(key) ~= 0
                           value = str2double(parts{2});
                           if key(1) == 'w'
                               scale_no = str2double(key(2));
                               obj.weights(scale_no) = value;
                           elseif key(1) == 'd'
                               door_no = str2double(key(2));
                               obj.doors(door_no) = value;
                           end
                       end
                   end
                end
            end
            readasync(obj.serialPort);
        end
    end
end
 

