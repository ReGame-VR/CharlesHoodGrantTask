function [h_timerinput] = timer_input(xloc, yloc, saveduration)

h_timerinput = uicontrol('Style', 'edit', 'fontsize', 13, 'String', num2str(saveduration),...
    'Units','normalized', 'Position', [xloc, yloc,.05,.025],'Enable','off','Callback',@callbackfn_timerinput); 
% @callbackfn = it calls the function, horizontal - vertical - side- width

end


% %Callback function
function callbackfn_timerinput(hObject, eventdata)

global saveduration h_timerinput Upper_time

saveduration = str2double(get(hObject,'String'));

Lower_time = 1/10;
if(saveduration > Upper_time) || (saveduration < Lower_time)
    saveduration = TimeGetter(Upper_time,Lower_time);
end

savetext = mat2str(saveduration);
set(h_timerinput,'String',savetext);


end


