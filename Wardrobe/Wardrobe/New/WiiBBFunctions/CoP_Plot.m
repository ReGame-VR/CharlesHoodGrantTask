% function [rx1,ry1,rx2,ry2] = CoP_Plot (textfile)


% figure

[frame,time,copx,copy,f1,f2,f3,f4] = textread(textfile);

% find the last index of the first trial
end_index = find(diff(frame)-1);

subplot('position',[.1  .1  .8  .7]);
            % plot -cog(2), i.e. -copy to match screen
%             plot(cog(1), -cog(2),'ro', 'MarkerFaceColor','r', 'MarkerSize', 16); axis([-22.5 22.5 -13 13]);

% plot figures
plot(copx(1:end_index)-mean(copx(1:end_index)),copy(1:end_index)-mean(copy(1:end_index)),'bo')
hold on
plot(copx((end_index+1):end)-mean(copx((end_index+1):end)),copy((end_index+1):end)-mean(copy((end_index+1):end)),'r')

% X and Y ranges
rx1 = range(copx(1:end_index));
ry1 = range(copy(1:end_index));
rx2 = range(copy((end_index+1):end));
ry2 = range(copx((end_index+1):end));

% end