function [h_timerbutton] = timer_button

h_timerbutton = uicontrol('Style', 'togglebutton', 'String', 'TIMED RECORDING',...
    'Units','normalized', 'Position', [.7,.95,.1,.05],'Enable','on','Callback',@callbackfn_timer); 
% @callbackfn = it calls the function, horizontal - vertical - side- width


% %Callback function
function callbackfn_timer(hObject, eventdata)

global go collecting h_timerinput h_startbutton flag_timed; 

button_state = get(hObject,'Value');
if button_state == 1
    % toggle button is pressed
    set(h_timerinput,'Enable','on');
    set(h_startbutton,'Style','pushbutton');
    flag_timed = 1;
elseif button_state == 0
    % toggle button is not pressed
    set(h_startbutton,'Style','togglebutton');
    set(h_timerinput,'Enable','off');
    flag_timed = 0;
end




