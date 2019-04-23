function [buttonPressed]= isNunchukButtonPressed(ButtonToCheck)
% usage:	      isButtonPressedNunchuk(ButtonToCheck)
%           	Possible Buttons :: C Z
% purpose:		Tests to see if a button is being pressed
% return:		boolean value (true(1) or false(0)) indicating if the button is pressed.
   
global wiimote;

wiimote.GetNunchukButtonsState();

% check if the button is pressed based on the ButtonToCheck input
switch(upper(ButtonToCheck))
    case 'C'
        buttonPressed = wiimote.NunchukButtons.C;
    case 'Z'
        buttonPressed = wiimote.NunchukButtons.Z;    
    otherwise
        % Bad input
        error('Bad input to isButtonPressed, must be one of the following: C Z');
end
     
end
