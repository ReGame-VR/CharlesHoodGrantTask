function [h_startbutton] = start_button

h_startbutton = uicontrol('Style', 'pushbutton', 'String', 'START RECORDING',...
    'Units','normalized', 'Position', [.4,.95,.1,.05],'Callback',@callbackfn_start); 
% @callbackfn = it calls the function, horizontal - vertical - side- width

end

% %Callback function
function callbackfn_start(source,eventdata)

uiresume
% set([hsttext huitext],'Visible','off');
global flag_saving flag_start go deletedfile; 

flag_saving = 1;
flag_start = 1;
deletedfile = 0;
go = 1;

hold off

end