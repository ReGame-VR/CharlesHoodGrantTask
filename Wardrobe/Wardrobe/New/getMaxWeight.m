function [zeroX, zeroY, XMaxWeight, XMinWeight, YMaxWeight, YMinWeight] = getMaxWeight(bb)

XMaxWeight = 0;
XMinWeight = 0;
YMaxWeight = 0;
YMinWeight = 0;
zeroX = 0;
zeroY = 0;
center = [0, 0];

uiwait(msgbox('Press OK when you are ready to begin calibration.','Wii Message','modal'))

waitTime = 5;
shiftErr = 0;
count = 1;

%DEFAULT SECTION
uiwait(msgbox('Please stand as still as possible.','Wii Message','modal'))
tic
while toc < 1
    CoP = bb.wm.GetBalanceBoardCoGState();
    center(count, :) = [CoP];
    count = count + 1;
end
average = mean(center);
zeroX = average(1)
zeroY = average(2)

%RIGHT SHIFT SECTION
while shiftErr == 0
    XMaxWeight = 0;
    uiwait(msgbox('Weight shift as far right as possible.','Wii Message','modal'))
    tic
    while toc < waitTime
        CoP = bb.wm.GetBalanceBoardCoGState();
        if CoP(1) > XMaxWeight
            XMaxWeight = CoP(1); 
        end
    end
    if abs(XMaxWeight) < 25
        shiftErr = 1;
    end
end
XMaxWeight
shiftErr = 0;

%LEFT SHIFT SECTION
while shiftErr == 0
    XMinWeight = 0;
    uiwait(msgbox('Weight shift as far left as possible.','Wii Message','modal'))
    tic
    while toc < waitTime
        CoP = bb.wm.GetBalanceBoardCoGState();
        if CoP(1) < XMinWeight
            XMinWeight = CoP(1);
        end
    end
    if abs(XMinWeight) < 25
        shiftErr = 1;
    end
end
XMinWeight
shiftErr = 0;

%FORWARD SHIFT SECTION
while shiftErr == 0
    YMaxWeight = 0;
    uiwait(msgbox('Weight shift as far forward as possible.','Wii Message','modal'))
    tic
    while toc < waitTime
        CoP = bb.wm.GetBalanceBoardCoGState();
        if (-CoP(2) > YMaxWeight)
            YMaxWeight = -CoP(2); 
        end
    end
    if abs(YMaxWeight) < 20
        shiftErr = 1;
    end
end
YMaxWeight
shiftErr = 0;

%BACKWARD SHIFT SECTION
while shiftErr == 0
    YMinWeight = 0;
    uiwait(msgbox('Weight shift as far backward as possible.','Wii Message','modal'))
    tic
    while toc < waitTime
        CoP = bb.wm.GetBalanceBoardCoGState();
        if -CoP(2) < YMinWeight
            YMinWeight = -CoP(2);
        end
    end
    YMinWeight
    if abs(YMinWeight) < 20
        shiftErr = 1;
    end
end
YMinWeight
shiftErr = 0;

uiwait(msgbox('Thank you!.','Wii Message','modal'))

end
