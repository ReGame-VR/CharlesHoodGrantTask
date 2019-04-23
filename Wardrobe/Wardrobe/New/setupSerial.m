function[ard] = setupSerial(comPort)
% It accept as the entry value, the index of the serial port
% Arduino is connected to, and as output values it returns the serial 
% element obj
% Initialize Serial object
ard = serial(comPort);
set(ard,'DataBits',8);
set(ard,'StopBits',1);
set(ard,'BaudRate',9600);
set(ard,'Parity','none');
fopen(ard);
a = 'b';

while (a~='a') 
    a=fread(ard,1,'uchar'); %Wait for the input from the arduino
end
if (a=='a')
    disp('Serial read'); %Once we get that input, display "Serial read" to the user and
end
fprintf(ard,'%c','g'); %Send the arduino a 'g' so it knows we are good to go
pause(.5)

end