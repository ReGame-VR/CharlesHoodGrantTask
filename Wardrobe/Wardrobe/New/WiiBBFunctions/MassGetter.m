function [Mass, quitgui] = MassGetter(Upper,Lower)
%% This function fetches an RPM value for the loop.

quitgui = 0; %don't quit by default

gotMass=0; % lets us into the loop
while gotMass == 0
    
    prompt = sprintf('Enter the mass that you will be using for calibration [kg]. \r Set the mass on the Balance Board. For best results, use a heavy, precise mass. '); % this text will be used in the input dialog
    Mass = inputdlg(prompt,'Mass',1,{'0'}); % open the input dialog
    
    if ~isempty(Mass)  
    Mass = str2num(cell2mat(Mass));
    else
        quitgui = 1;
        break
    end
    
    
    if (Mass <= Lower)   % check if this is out of the reasonable range
        
        % if so, complain
        uiwait(msgbox('Enter a larger number','Mass too low','modal'));
    elseif (Mass > Upper)
        uiwait(msgbox('Enter a smaller number','Mass too high','modal'));
        
    else % then we are all set       
        
        gotMass=1;
        
    end
end
end