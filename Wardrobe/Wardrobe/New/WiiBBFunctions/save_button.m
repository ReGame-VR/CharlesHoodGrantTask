function save_button

startbutton = uicontrol('Style', 'togglebutton', 'String', 'START SAVING',...
    'Units','normalized', 'Position', [.4,.9,.1,.1],'Callback',@callbackfn_start); 
% @callbackfn = it calls the function, horizontal - vertical - side- width


% %Callback function
function callbackfn_start(source,eventdata)

button_state = get(hObject,'Value');
if button_state == 1
    % toggle button is pressed
elseif button_state == 0
    % toggle button is not pressed
end

uiresume
% set([hsttext huitext],'Visible','off');
global flag_saving go; 
flag_saving = 1;

