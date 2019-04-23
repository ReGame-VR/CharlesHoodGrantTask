% function keyPressCallback(source,eventdata)
function keyPressCallback(~,eventDat, cogg)


      % determine the key that was pressed
      keyPressed = eventDat.Key;

      if strcmpi(keyPressed,'space')
        COPdata1 = fopen('COPcoordinates.txt','a'); %open the file in append mode
        fprintf(COPdata1,'%3.2f\t\t%3.2f\n',cogg(1), -cogg(2));
        fclose(COPdata1);
      end
  end