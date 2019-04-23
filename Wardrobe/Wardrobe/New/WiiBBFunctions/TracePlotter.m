%-- 2/17/11  8:51 AM --%
% help textread
[frame,time,copx,copy,f1,f2,f3,f4] = textread('test1.txt','%d%d%d%d%d%d%d%d','delimiter','\t');
[frame,time,copx,copy,f1,f2,f3,f4] = textread('test1.txt','%f%f%f%f%f%f%f%f','delimiter','\t');
whos
figure
plot(time,[copx copy])
figure
plot(time,'*')
grid on
[frame,time,copx,copy,f1,f2,f3,f4] = textread('test2.txt','%f%f%f%f%f%f%f%f','delimiter','\t');
figure
plot(time,'*')
tdiff=diff(time)
length(time)
length(tdiff)
max(tdiff)
range(tdiff)
min(tdiff)
range(copx)
range(copy)
tdiff
tdiff(1:100)
range(copy)
range(copx)
[frame,time,copx,copy,f1,f2,f3,f4] = textread('test2.txt','%f%f%f%f%f%f%f%f','delimiter','\t');
range(copx)
range(copy)
figure
plot(copx(1:363),copy(1:363),'b')
hold onn
hold on
plot(copx(364:end),copy(364:end),'r')
figure
plot(copx(1:363)-mean(copx(1:363)),copy(1:363)-mean(copy(1:363)),'b')
hold on
plot(copx(364:end)-mean(copx(364:end)),copy(364:end)-mean(copy(364:end)),'r')
range(copy(364:end))
range(copx(364:end))
CoP-Plot
CoP_Plot