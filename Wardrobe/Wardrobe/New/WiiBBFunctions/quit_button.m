function [h_quitbutton] = quit_button

h_quitbutton = uicontrol('Style', 'pushbutton','String', 'QUIT',...
    'Units','normalized','Position', [.8,.95,.1,.05],'Callback',@callbackfn_quit);
% @callbackfn = it calls the function, horizontal - vertical - side-

end

% %Callback function
function callbackfn_quit(source,eventdata)
global go quitgui

uiresume

go = 0;
quitgui=1;


end