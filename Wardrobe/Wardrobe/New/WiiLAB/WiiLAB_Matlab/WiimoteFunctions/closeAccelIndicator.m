function closeAccelIndicator()
%	usage:      closeAccelIndicator (obj)
%	purpose:	Disables the AccelIndicator and deletes the current
%               AccelIndicator

global wiimote;

wiimote.CloseAccelIndicator();

