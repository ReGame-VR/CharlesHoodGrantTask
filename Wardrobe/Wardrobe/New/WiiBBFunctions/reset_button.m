function [h_resetbutton] = reset_button

h_resetbutton = uicontrol('Style', 'pushbutton', 'Enable', 'off', 'String', 'RESET',...
    'Units','normalized', 'Position', [.6,.95,.1,.05],'Callback',@callbackfn_reset); 
% @callbackfn = it calls the function, horizontal - vertical - side- width

end

% %Callback function
function callbackfn_reset(source,eventdata)

uiresume
% set([hsttext huitext],'Visible','off');
global  flag_reset go deletedfile reset h_timerinput 

flag_reset = 1;
deletedfile = 0;
go = 1;
set(h_timerinput,'Enable','off')

reset = 1;

hold off

if exist('trace_ind', 'file')
    load trace_ind
    trace_ind = trace_ind + 1;
    save trace_ind trace_ind
end

uiresume

end