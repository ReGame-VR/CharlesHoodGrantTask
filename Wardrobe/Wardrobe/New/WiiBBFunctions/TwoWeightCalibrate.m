function [C, quitgui, sensor_avg_upright ] = TwoWeightCalibrate (bb)

% This function is used for a quick calibration of the balance board.

recal = 0; % we have not recalibrated the balance board
quitgui = 0;

% dummy values, so that they are always assigned
C = 0;
sensor_avg_upright = 0;

uiwait(msgbox('Set the Balance Board upright. Press OK when you are ready to begin calibration.','Wii Message','modal'))

Lower=0;
Upper=100;

while recal == 0
    
    [Mass, quitgui] = MassGetter(Upper,Lower);
    
    if quitgui == 1
        break
    end


%     uiwait(msgbox('Set the mass on the balance board. Press OK to continue.','Wii Message','modal'))
    sensor_mass= Calibrate_BB (bb);
    maxval = max(max(sensor_mass));


    if (maxval < 10)

        recal = 0;

        answer = questdlg('You do not seem to have placed a heavy enough mass on the balance board. Would you like to try again?', ...
            'User Error', 'Yes', 'No','Yes');
        switch answer,
            case 'Yes',
                quitgui = 0;
            case 'No',
                quitgui = 1;
        end

    else

        recal = 1;

    end

end

recal = 0;
while recal == 0
    
    if quitgui == 1
        break
    end
    
    uiwait(msgbox('Remove the mass. Do not stand on the balance board until the GUI loads. Press OK to finish calibration and continue.','Wii Message','modal'))
    sensor_avg_upright = Calibrate_BB (bb);
    max_upright = max(sensor_avg_upright);
    
    if (max_upright > 5)

        recal = 0;

        answer = questdlg('You do not seem to have removed the mass from the balance board. Would you like to try again?', ...
            'User Error', 'Yes', 'No','Yes');
        switch answer,
            case 'Yes',
                quitgui = 0;
            case 'No',
                quitgui = 1;
                
        end

    else
        recal = 1;
        
        C = (sensor_mass-sensor_avg_upright)/Mass*4; % calibration factor
        C = [sum(C)/4,sum(C)/4,sum(C)/4,sum(C)/4]; % average them
        
    end
end


end