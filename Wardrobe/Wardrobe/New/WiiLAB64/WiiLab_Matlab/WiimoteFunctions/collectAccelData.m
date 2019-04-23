function [X Y Z T] = collectAccelData(time, rate)
%  usage:    collectAccelData(time, rate)
%  purpose:  This method will collect the acceleration data from the
%            Wiimote and store it into the X Y Z T variables as arrays. The
%            time parameter indicates how long (in seconds) the program should collect
%            the data and the rate parameter indicates the time between
%            each data point.
%  example:  time = 60
%            rate = 1
%            The program will record data for the next 60 seconds recording
%            the acceleration every second. The result will be arrays with
%            60 data points plus 1 for the 0 point (61 total)


global wiimote;


switch (nargin)
    case 0
        time = 30;
        rate = .1;
    case 1
        rate = .1;
    otherwise
end

% length
points = time/rate;

% intialize the arrays    
X = zeros(1, points);
Y = zeros(1, points);
Z = zeros(1, points);
T = zeros(1, points);

uiwait(msgbox('Data collection will begin after pressing OK.', ...
                'Begin Data Collection', 'help'));

timer = 0;

for i = 1 : points + 1
    
    wiimote.GetAccelState();
    
    X(i) = wiimote.Accel.X;
    Y(i) = wiimote.Accel.Y;
    Z(i) = wiimote.Accel.Z;
    T(i) = timer;
    
    timer = timer + rate;
        
    pause(rate);    
    
end

