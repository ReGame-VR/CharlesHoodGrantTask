function sensor_avg = Calibrate_BB (bb)
 % for calibrating the balance board

n=1000;

% initialize
F = zeros(n,4);
Fdiff = zeros(n-1,1);
Change = zeros(n-1,1);
t = zeros(n,1);
Fadd = zeros(n,1);
sensor_avg = zeros(1,4);


t1 = tic; % start stopwatch

% take a ton of data points as fast as the computer can (faster than the Wii sends
% them)
for ii = 1:n
    F(ii,:) = bb.wm.GetBalanceBoardSensorState(); % sensor readings
    t(ii) = toc(t1); % times elapsed
end

time = toc(t1); % end stopwatch



for ii = 1:4 % for each column, find the differences
Fdiff(:,ii) = diff(F(:,ii));
end

for ii = 1:n-1 % for each row, find whether or not there is a difference
Change(ii,1) = (Fdiff(ii,1) || Fdiff(ii,2)) || (Fdiff(ii,3) || Fdiff(ii,4));
end

changes = find(Change)+1; % find the indices where new samples were taken
% tgaps = diff(t(changes)) % find the differences between the times when new samples were taken
% 
% longest_period = max(tgaps) % longest time between distinct samples
% 
% 
% 
 Samples = sum(Change); % total number of *distinct* samples
% 
% avg_freq = Samples/time % calculate the sampling frequency
% min_freq = 1/longest_period
% 
% time % display the time

for ii=1:4
    sensor_avg(ii)=sum(F(changes,ii))/Samples;
end

end



