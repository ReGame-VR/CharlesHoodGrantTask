function stop_button

stopbutton = uicontrol('Style', 'pushbutton','String', 'STOP',...
    'Units','normalized', 'Position', [.5,.95,.1,.05],'Callback',@callbackfn_stop);
% @callbackfn = it calls the function, horizontal - vertical - side-


% %Callback function
function callbackfn_stop(source,eventdata)

% set([hsttext huitext],'Visible','off');
global go;
uiwait

