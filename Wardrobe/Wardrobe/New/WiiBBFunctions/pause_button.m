function [h_pausebutton] = pause_button

h_pausebutton = uicontrol('Style', 'togglebutton', 'String', 'SHOW RESULTS',...
    'Units','normalized', 'Position', [.5,.95,.1,.05],'Enable','off','Callback',@callbackfn_pause); 
% @callbackfn = it calls the function, horizontal - vertical - side- width


% %Callback function
function callbackfn_pause(hObject, eventdata, handles)

global go collecting; 

button_state = get(hObject,'Value');
if button_state == 1
    % toggle button is pressed
    go = 0;
    collecting = 0;
elseif button_state == 0
    % toggle button is not pressed
    go = 1;
    collecting = 1;
    hold off
end




