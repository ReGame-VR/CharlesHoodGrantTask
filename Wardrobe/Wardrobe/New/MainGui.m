classdef MainGui < handle
    %MAINGUI GUI for displaying balance board data
    %   
    
    properties (SetAccess = private, GetAccess = private)

        fontSize = 10
        
        % Figure handle
        fig

        % Text display handles
        copDisplayX
        copDisplayY
        bbDisplayBottomLeft
        bbDisplayBottomRight
        bbDisplayTopLeft
        bbDisplayTopRight
        bbDisplayTotal
        
        % Handle to plot axes
        bbCopAxes
        weightAxes
        
        % Selected Target Area
        % [xMin xMax yMin yMax]
        cogTargetArea = [-5 5 -5 5]
        targetThreshold = 3
        
        door1StatusLed
        door2StatusLed
        drawerStatusLed
        currentTargetDisplay
        loadCellReadingDisplay
        target = 0
        
    end
    
    methods
        
        function obj = MainGui()
            
            % Positions on plot
            xpos = [-0.1 0.15 0.3 0.26 0.7 0.5];
            ypos = [0.95 0.8 0.6 0.45 0.3 0.15 0];
            
            % Create GUI figure %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            [obj.fig] = figure(...
                'Visible', 'off', ...
                'color', [0.8 0.8 0.8], ...
                'units', 'normalized', ...
                'outerposition', [0 0 1 1], ...
                'Name','Operator GUI');
            
            % Create balance board panel %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            bbPanel = uipanel(...
                'Parent', obj.fig, ...
                'Title','Balance Board', ...
                'FontSize',obj.fontSize, ...
                'Position',[.05 .05 .4 .75]);
            
            
            % Set up COP plot %%%%%%%%%%
            obj.bbCopAxes = axes(...
                'Parent', bbPanel, ...
                'Units', 'Normalized', ...
                'Position', [0.05, 0.05, 0.9, 0.6]);
            
            
            grid(obj.bbCopAxes, 'on');
            title(obj.bbCopAxes, 'Center Of Pressure');
            
            % Set up X axis
            xlabel(obj.bbCopAxes, 'X [cm]');
            xlim(obj.bbCopAxes, [-22.5 22.5]);
            
            % Set up y axis
            ylabel(obj.bbCopAxes, 'Y [cm]');
            ylim(obj.bbCopAxes, [-13 13]);
            set(obj.bbCopAxes, 'fontsize', obj.fontSize);
            
            % This causes subsequent calls to `plot` to replace the series
            % on the existing plot, without resetting the labels, limits,
            % etc.
            set(obj.bbCopAxes, 'NextPlot', 'replaceChildren');
            
            
            % Create labels %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            h = subplot('position', [.1  .8  .8  .2]);
            set(h, 'visible','off','Units', 'normalized');
            
            % COP X and Y labels
            text(xpos(1), ypos(1), 'CoP X [cm] = ','fontsize', obj.fontSize);
            text(xpos(1), ypos(2), 'CoP Y [cm] = ','fontsize', obj.fontSize);
            
            % Force Sensor values
            text(xpos(1), ypos(3), 'Bottom Left force [kgf] = ','fontsize', obj.fontSize);
            text(xpos(1), ypos(4), 'Bottom Right force [kgf] = ','fontsize', obj.fontSize);
            text(xpos(1), ypos(5), 'Top Left force [kgf] = ','fontsize', obj.fontSize);
            text(xpos(1), ypos(6), 'Top Right force [kgf] = ','fontsize', obj.fontSize);
            text(xpos(1), ypos(7), 'Total force [kgf] = ','fontsize', obj.fontSize);
            
            % Create Value displays %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            
            [obj.copDisplayX] = text(xpos(2), ypos(1), 'xx','fontsize', obj.fontSize);
            [obj.copDisplayY] = text(xpos(2), ypos(2), 'xx','fontsize', obj.fontSize);
            
            obj.bbDisplayBottomLeft = text(xpos(3), ypos(3), 'xx','fontsize', obj.fontSize);
            obj.bbDisplayBottomRight = text(xpos(3), ypos(4), 'xx','fontsize', obj.fontSize);
            obj.bbDisplayTopLeft = text(xpos(3), ypos(5), 'xx','fontsize', obj.fontSize);
            obj.bbDisplayTopRight = text(xpos(3), ypos(6), 'xx','fontsize', obj.fontSize);
            obj.bbDisplayTotal = text(xpos(3), ypos(7), 'xx','fontsize', obj.fontSize);
            
            
            
            % Create Wardrobe state panel %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
            wardPanel = uipanel(...
                'Parent', obj.fig, ...
                'Title','Wardrobe', ...
                'FontSize',obj.fontSize, ...
                'Position',[.55 .05 .4 .75]);
            
            
            
            % Set up load cells plot %%%%%%%%%%
            obj.weightAxes = axes('Parent', wardPanel, ...
                'Units', 'Normalized', ...
                'Position', [0.1, 0.05, 0.85, 0.6]);
            grid(obj.weightAxes, 'on');
            title(obj.weightAxes, 'Load Cells');
            
            % Set up X axis
            xlabel(obj.weightAxes, 'Scale #');
            xlim(obj.weightAxes, [0 8]);
            
            % Set up y axis
            ylabel(obj.weightAxes, 'Weight');
            ylim(obj.weightAxes, [-1000 20000000]);
            
            set(obj.weightAxes, 'fontsize', 13);
            
            % This causes subsequent calls to `plot` to replace the series
            % on the existing plot, without resetting the labels, limits,
            % etc.
            set(obj.weightAxes, 'NextPlot', 'replaceChildren');
            
            % Selected target display %%%%
            uicontrol(wardPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.55 0.855 0.2 0.05], ...
                'String', 'Current Target:', ...
                'fontsize', 10, ...
                'HorizontalAlignment', 'left' ...
            );

            obj.currentTargetDisplay = uicontrol(wardPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.76 0.855 0.2 0.05], ...
                'String', 'None', ...
                'fontsize', 10, ...
                'HorizontalAlignment', 'left' ...
            );
        
            % Current Weight Display %%%%
            uicontrol(wardPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.55 0.825 0.25 0.05], ...
                'String', 'Load Cell Reading:', ...
                'fontsize', 10, ...
                'HorizontalAlignment', 'left' ...
            );

            obj.loadCellReadingDisplay = uicontrol(wardPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.76 0.825 0.2 0.05], ...
                'String', '', ...
                'fontsize', 10, ...
                'HorizontalAlignment', 'left' ...
            );
            
            
            % Door state display %%%%
            uicontrol(wardPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.12 0.825 0.2 0.05], ...
                'String', 'Left Door', ...
                'fontsize', 10, ...
                'HorizontalAlignment', 'left' ...
            );
            
            uicontrol(wardPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.12 0.775 0.2 0.05], ...
                'String', 'Right Door', ...
                'fontsize', 10, ...
                'HorizontalAlignment', 'left' ...
            );
            
            uicontrol(wardPanel, ...
                'Style', 'Text', ...
                'Units', 'Normalized', ...
                'Position', [0.12 0.725 0.2 0.05], ...
                'String', 'Drawer Door', ...
                'fontsize', 10, ...
                'HorizontalAlignment', 'left' ...
                );
            
            % Create door state readout %%%%%%%%%%
            obj.door1StatusLed = uipanel( ...
                'Parent', wardPanel, ...
                'Position', [0.05 0.85 0.05 0.025], ...
                'Units', 'Normalized', ...
                'BackgroundColor', 'red', ...
                'BorderType', 'none' ...
            );
        
            obj.door2StatusLed = uipanel( ...
                'Parent', wardPanel, ...
                'Position', [0.05 0.8 0.05 0.025], ....
                'BackgroundColor', 'red', ...
                'BorderType', 'none' ...
            );
       
            obj.drawerStatusLed = uipanel( ...
                'Parent', wardPanel, ...
                'Position', [0.05 0.75 0.05 0.025], ...
                'BackgroundColor', 'red', ... 
                'BorderType', 'none' ...
            );
        
            set(obj.fig, 'Visible', 'on'); 
        end
        
        function delete(obj)
            if obj.fig.isvalid
                close(obj.fig);
            end
        end
        
        function setTarget(obj, target)
            obj.target = target;
            if target > 0
                set(obj.currentTargetDisplay, 'String', target);
            else
                set(obj.currentTargetDisplay, 'String', '');
            end
        end
        
        function setBalanceBoardState(obj, cog, sensorState, color)
            
            % Calculate total weight on balance board.
            weight = sum(sensorState);
            
            % Update text displays
            set(obj.copDisplayX, 'String', cog(1));
            set(obj.copDisplayY, 'String', -cog(2));
            set(obj.bbDisplayBottomLeft, 'String', sensorState(1));
            set(obj.bbDisplayBottomRight, 'String', sensorState(2));
            set(obj.bbDisplayTopLeft, 'String', sensorState(3));
            set(obj.bbDisplayTopRight, 'String', sensorState(4));
            set(obj.bbDisplayTotal, 'String', weight);
            
            % Set COP plot marker size based on total weight
            copSize = weight / 1.5;
            if copSize < 1
                copSize = 1;
            end
            
            % Convert color index to display color
            colorTable = {'green', 'yellow', 'red'};
            colorString = colorTable{color};
            
            % Plot COP on display
            plot(obj.bbCopAxes, cog(1), -cog(2),'o', ...
                'Color', colorString, ....
                'MarkerFaceColor', colorString, ...
                'MarkerSize', copSize);
        end
        
        function setWardrobeState(obj, doorStates, loadCellStates)
            bar(obj.weightAxes, loadCellStates);
            
            colors = {'red' 'green'};
            
            set(obj.door1StatusLed, 'BackgroundColor', colors{doorStates(1) + 1});
            set(obj.door2StatusLed, 'BackgroundColor', colors{doorStates(2) + 1});
            set(obj.drawerStatusLed, 'BackgroundColor', colors{doorStates(3) + 1});
            
            if obj.target > 0
                set(obj.loadCellReadingDisplay, 'String', loadCellStates(obj.target));
            else
                set(obj.loadCellReadingDisplay, 'String', '');
            end
        end
    
        function reset(obj)
            obj.setTarget(0);
            obj.setBalanceBoardState([0 0], [0 0 0 0], 1);
            obj.setWardrobeState([0 0 0], [0 0 0 0 0 0 0]);
        end
        
        function close(obj)
            if obj.fig.isvalid
                close(obj.fig);
            end
        end
    end
end

