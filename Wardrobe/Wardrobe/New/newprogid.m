function convertedProgID = newprogid(progID)
% Copyright 1984-2002 The MathWorks, Inc.
% $Revision: 1.3 $ $Date: 2002/09/05 19:16:38 $

%% Initialize temporary and output variables
tempProgID = progID;
convertedProgID = [];

% While we haven't got to the last '.' token keep looking ...
while ~isempty(tempProgID)
    [tempStr tempProgID] = strtok(tempProgID, '.');    
    
    % If the delimited string starts with a letter concatenate it as is and add
    % the '.' token since it is not part of the delimited string.
    % If it is a number add the "TMW_" token and the '.'. 
    if (isletter(tempStr(1)))
       convertedProgID = [convertedProgID tempStr];
    else
        convertedProgID = [convertedProgID 'TMW_TMW' tempStr];
        
    end
 
    % Don't add the '.' if it is the last token in the string
    if (~isempty(tempProgID))
       convertedProgID =  [convertedProgID '.'];    
    end   

end
%replace empty space with TMW_TMW_SP
convertedProgID = regexprep(convertedProgID, ' ', 'TMW_SP_TMW');
