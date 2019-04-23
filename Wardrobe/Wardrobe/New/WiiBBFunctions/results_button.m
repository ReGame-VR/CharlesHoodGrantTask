function [h_resultsbutton] = results_button

h_resultsbutton = uicontrol('Style', 'pushbutton', 'String', 'SHOW RESULTS',...
    'Units','normalized', 'Position', [.5,.95,.1,.05],'Enable','off','Callback',@callbackfn_results);
% @callbackfn = it calls the function, horizontal - vertical - side- width

end

%%Callback function
function callbackfn_results(hObject, eventdata, handles)

global go collecting enable_results h_resultsbutton;

button_state = get(hObject,'Value');

% toggle button is pressed
go = 0;
collecting = 0;

enable_results = 0;

set(h_resultsbutton, 'String', 'Next')

if exist('trace_ind.mat','file')
    load trace_ind
    trace_ind = trace_ind +1;
    save trace_ind trace_ind
end

end





