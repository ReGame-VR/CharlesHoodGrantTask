function app_filename = NameGetter
% This function fetches a name for a text file.

gotaname=0; % lets us into the loop
while gotaname == 0
    
    prompt = sprintf('Enter a name for the file'); % this text will be used in the input dialog
    filename = char(inputdlg(prompt,'Name',1,{'File Name'})); % open the input dialog
    app_filename = strcat(filename,'.txt'); % append the filename with the filetype
    
    if exist(app_filename,'file') > 0 % check if there is a file of that name
        
        % prompt with option to everwrite
        overwrite = questdlg('That filename is in use. Overwrite?','Overwrite');
        
        switch overwrite
            
            case 'No' % then try again
                continue
                
            case 'Yes' % then we are all set
                gotaname = 1;
        end
        
    else % then we are all set
        gotaname=1;
    end
end
end