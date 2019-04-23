function [h_startbutton] = start_button_toggle

h_startbutton = uicontrol('Style', 'togglebutton', 'String', 'START RECORDING',...
    'Units','normalized', 'Position', [.4,.95,.1,.05],'Callback',@callbackfn_start); 
% @callbackfn = it calls the function, horizontal - vertical - side- width

end

% %Callback function
function callbackfn_start(hObject,eventdata)

uiresume
% set([hsttext huitext],'Visible','off');
global flag_saving flag_start flag_stop go deletedfile h_startbutton ...
    h_timerbutton h_timerinput; 

style = get(h_startbutton,'Style');

switch style
    case 'pushbutton'
        % if style == 'pushbutton'
        flag_saving = 1;
        flag_start = 1;
        deletedfile = 0;
        go = 1;
        hold off
        
        set(h_timerbutton,'Enable','off');
        set(h_timerinput,'Enable','off');

    case 'togglebutton'
        % elseif style == 'togglebutton'

        button_state = get(hObject,'Value');
        if button_state == 1
            % toggle button is pressed
            set(h_timerbutton,'Enable','off')
            set(h_timerinput,'Enable','off')
            set(h_startbutton, 'String', 'STOP RECORDING');
            flag_saving = 1;
            flag_start = 1;
            deletedfile = 0;
            go = 1;
            hold off
        elseif button_state == 0
            % toggle button is not pressed
            set(h_timerinput,'Enable','off');
            set(h_startbutton, 'String', 'START RECORDING');
            flag_stop = 1;
        end
end

end