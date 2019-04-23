function initializeAccelIndicator(obj, velDamping)
%	usage:      initializeAccelIndicator (obj, velDamping)
%	purpose:	Enables the AccelIndicator that is tethered to this obj.
%               The velDamping variable is used to keep the tethered
%               centered on the object. The default value is 1 (ie. the
%               object is moved based directly off the acceleration value
%               with no scaling)

global wiimote;

if(nargin < 2)
    velDamping = 1;
end

wiimote.InitializeAccelIndicator(obj, velDamping);

