function Time = TimeGetter(Upper,Lower)
%% This function fetches an RPM value for the loop.

gotTime=0; % lets us into the loop
while gotTime == 0
    
    prompt = sprintf('Enter a number for save duration [s]'); % this text will be used in the input dialog
    Time = str2num(cell2mat(inputdlg(prompt,'Save Duration',1,{'0'}))); % open the input dialog
    
    if (Time < Lower)   % check if this is out of the reasonable range
        
        % if so, complain
        uiwait(msgbox('Enter a larger number','Duration Too Short','modal'));
    elseif (Time > Upper)
        uiwait(msgbox('Enter a smaller number','Duration Too Long','modal'));
        
    else % then we are all set       
        
        gotTime=1;
        
    end
end
end