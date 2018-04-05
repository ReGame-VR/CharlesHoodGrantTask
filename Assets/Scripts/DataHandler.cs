using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;
using System.IO;

/// <summary>
/// Writes a line of data after every trial, giving information on the trial.
/// </summary>
public class DataHandler : MonoBehaviour {

    // The interval of time before a touch that the post-processing algorithm uses.
    // If this is two seconds, for example, the algorithm will analyze the two
    // seconds preceding a target touch with greater detail.
    private float precedingInterval = 2.0f;

    private string pid = GlobalControl.Instance.participantID;

    // Floats used in the calculation for preceding touch data. 
    // These will increment depending on how long the person
    // spent in the color during the interval preceding a target touch.
    private float itemGreenTime = 0f;
    private float itemYellowTime = 0f;
    private float itemRedTime = 0f;

    // stores the data for writing to file at end of task
    List<Data> data = new List<Data>();

    // stores the continuous data (cop, com, etc) at end of task
    List<ContinuousData> continuousData = new List<ContinuousData>();

	/// <summary>
    /// Subscribe to data writing events.
    /// </summary>
	void Awake () {
        Task.OnRecordData += recordTrial;
        Task.OnRecordContinuousData += recordContinuousTrial;
	}

    /// <summary>
    /// Write all data to a file and unsubscribe from data writing event.
    /// </summary>
    void OnDisable()
    {
        // First, analyze the data to find interesting information 
        // about the two seconds leading to target touch
        List<PostProcessingData> ppData = CalculatePostProcessingData();

        // Next, write the trial data to file and add some of the info
        // calculated in post-processing
        WriteTrialFile(ppData);

        // Finally, write the file that displays continuous CoP/CoM
        WriteContinuousFile();
    }

    // Records trial data
    private void recordTrial(string participantId, bool rightHanded, bool isRotation, int trialNum, 
            float time, int targetNum, float targetTime,
            bool weightShiftSuccess, bool buttonSuccess, bool isRandomSequence,
            Vector2 weightPosn, float COPTotalPath, float trialScore,
            float cumulativeScore, float curGreenTime, float curYellowTime, float curRedTime)
    {
        data.Add(new Data(participantId, rightHanded, isRotation, trialNum, time,
            targetNum, targetTime, weightShiftSuccess, buttonSuccess,
            isRandomSequence, weightPosn, COPTotalPath, trialScore, cumulativeScore, curGreenTime, curYellowTime, curRedTime));
    }

    // Records the continuous data like CoP and CoM to the list of data
    private void recordContinuousTrial(string participantId, float time, Vector2 CoPposition, Target.posnIndicator curColor,
            Vector3 CoMposition, int targetNum, int trialNum)
    {
        continuousData.Add(new ContinuousData(participantId, time, CoPposition, curColor, CoMposition, targetNum, trialNum));
    }

    /// <summary>
    /// A class that stores info on each trial relevant to data recording. Every field is
    /// public readonly, so can always be accessed, but can only be assigned once in the
    /// constructor.
    /// </summary>
    class Data
    {
        public readonly string participantId;
        public readonly bool rightHanded;
        public readonly bool isRotation;


        public readonly int trialNum; // which trial it is
        public readonly float time; // global time
        public readonly int targetNum; // the index of the randomly selected target, ONE INDEXED
        public readonly float targetTime; // time to activate target - 10 if unsuccessful
        public readonly bool weightShiftSuccess; // did they shift to correct location?
        public readonly bool buttonSuccess; // did they activate target while in correct location?
        public readonly bool isRandomSequence; // are the targets selected randomly or from a set sequence
        public readonly Vector2 weightPosn; // weight posn on balance board 
                                            // - when target hit OR when time runs out
        public readonly float COPTotalPath; // The total distance the user's COB traveled during the task
        public readonly float trialScore; // the score of the target touch
        public readonly float cumulativeScore; // The total score for the session

        public readonly float curGreenTime; // the time spent in green for current target
        public readonly float curYellowTime; // ''' yellow ''''
        public readonly float curRedTime; // '''' red ''''

        public float precedingGreenTime; // the time spent in green for the 2 seconds preceding touch
        public float precedingYellowTime; // ''' yellow ''''
        public float precedingRedTime; // '''' red ''''

        public Data(string participantId, bool rightHanded, bool isRotation, int trialNum, 
            float time, int targetNum, float targetTime, 
            bool weightShiftSuccess, bool buttonSuccess, bool isRandomSequence, 
            Vector2 weightPosn, float COPTotalPath, float trialScore,
            float cumulativeScore, float curGreenTime, float curYellowTime, float curRedTime)
        {
            this.participantId = participantId;
            this.rightHanded = rightHanded;
            this.isRotation = isRotation;

            this.trialNum = trialNum;
            this.time = time;
            this.targetNum = targetNum;
            this.targetTime = targetTime;
            this.weightShiftSuccess = weightShiftSuccess;
            this.buttonSuccess = buttonSuccess;
            this.isRandomSequence = isRandomSequence;
            this.weightPosn = weightPosn;
            this.COPTotalPath = COPTotalPath;
            this.trialScore = trialScore;
            this.cumulativeScore = cumulativeScore;

            this.curGreenTime = curGreenTime;
            this.curYellowTime = curYellowTime;
            this.curRedTime = curRedTime;

            // These values are not assigned in the constructor because
            // the data must be analyzed before they can be calculated.
            this.precedingGreenTime = 0f;
            this.precedingYellowTime = 0f;
            this.precedingRedTime = 0f;

        }

        // Add the calculated preceding times to the trial data
        public void AddPrecedingTimes(float precedingGreen, float precedingYellow, float precedingRed)
        {
            this.precedingGreenTime = precedingGreen;
            this.precedingYellowTime = precedingYellow;
            this.precedingRedTime = precedingRed;
        }
    }

    /// <summary>
    /// A class that stores continuous data gathered during the task, such as continuous
    /// CoP position and CoM position. Every field is
    /// public readonly, so can always be accessed, but can only be assigned once in the
    /// constructor.
    /// </summary>
    class ContinuousData
    {
        public readonly string participantId;
        public readonly float time; // global time
        public readonly Vector2 CoPposition; // current CoP position
        public readonly Target.posnIndicator curColor; // What is the current color of the target?
        public readonly Vector3 CoMposition; // current CoM position
        public readonly int targetNum; // current target number (one-indexed)
        public readonly int trialNum; // current trial number

        public ContinuousData(string participantId, float time, Vector2 CoPposition, Target.posnIndicator curColor,
            Vector3 CoMposition, int targetNum, int trialNum)
        {
            this.participantId = participantId;
            this.time = time;
            this.CoPposition = CoPposition;
            this.curColor = curColor;
            this.CoMposition = CoMposition;
            this.targetNum = targetNum;
            this.trialNum = trialNum;
        }
    }

    /// <summary>
    /// Writes the Trial File to a CSV
    /// </summary>
    private void WriteTrialFile(List<PostProcessingData> ppData)
    {

        AddPostProcessingToTrialData(ppData);

        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid);
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "/TrialData" + pid + ".csv"))
        {
            Debug.Log("Writing trial data to file");
            // write header
            CsvRow header = new CsvRow();
            header.Add("Participant ID");
            header.Add("Right Handed?");
            header.Add("Rotating Targets?");
            header.Add("Trial Number");
            header.Add("Global Time");
            header.Add("Target Number");
            header.Add("Target Time");
            header.Add("Weight Shift Success?");
            header.Add("Hit Success?");
            header.Add("Random Sequence?");
            header.Add("X Coord of COP");
            header.Add("Y Coord of COP");
            header.Add("COP Total Path");
            header.Add("Trial Score");
            header.Add("Cumulative Score");
            header.Add("Green Time");
            header.Add("Yellow Time");
            header.Add("Red Time");

            header.Add("Preceding Green Time");
            header.Add("Preceding Yellow Time");
            header.Add("Preceding Red Time");
            writer.WriteRow(header);

            // write each line of data
            foreach (Data d in data)
            {
                CsvRow row = new CsvRow();

                row.Add(d.participantId);
                if (d.rightHanded)
                {
                    row.Add("YES");
                }
                else
                {
                    row.Add("NO");
                }
                if (d.isRotation)
                {
                    row.Add("YES");
                }
                else
                {
                    row.Add("NO");
                }
                row.Add(d.trialNum.ToString());
                row.Add(d.time.ToString());
                row.Add(d.targetNum.ToString());
                row.Add(d.targetTime.ToString());
                if (d.weightShiftSuccess)
                {
                    row.Add("YES");
                }
                else
                {
                    row.Add("NO");
                }
                if (d.buttonSuccess)
                {
                    row.Add("YES");
                }
                else
                {
                    row.Add("NO");
                }
                if (d.isRandomSequence)
                {
                    row.Add("YES");
                }
                else
                {
                    row.Add("NO");
                }
                row.Add(d.weightPosn.x.ToString());
                row.Add(d.weightPosn.y.ToString());
                row.Add(d.COPTotalPath.ToString());
                row.Add(d.trialScore.ToString());
                row.Add(d.cumulativeScore.ToString());
                row.Add(d.curGreenTime.ToString());
                row.Add(d.curYellowTime.ToString());
                row.Add(d.curRedTime.ToString());

                row.Add(d.precedingGreenTime.ToString());
                row.Add(d.precedingYellowTime.ToString());
                row.Add(d.precedingRedTime.ToString());

                writer.WriteRow(row);
            }
        }

        Task.OnRecordData -= recordTrial;

    }

    /// <summary>
    /// Writes the file that has the continuous data (CoP, CoM) to a file
    /// </summary>
    private void WriteContinuousFile()
    {
        // Write all entries in data list to file
        Directory.CreateDirectory(@"Data/" + pid);
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/" + pid + "/ContinuousData" + pid + ".csv"))
        {
            Debug.Log("Writing continuous data to file");
            // write header
            CsvRow header = new CsvRow();
            header.Add("Participant ID");
            header.Add("Global Time");
            header.Add("CoP X");
            header.Add("CoP Y");
            header.Add("Current Color");
            header.Add("CoM X");
            header.Add("CoM Y");
            header.Add("CoM Z");
            header.Add("Target Number");
            header.Add("Trial Number");

            writer.WriteRow(header);

            // write each line of data
            foreach (ContinuousData d in continuousData)
            {
                CsvRow row = new CsvRow();

                row.Add(d.participantId);
                row.Add(d.time.ToString());
                row.Add(d.CoPposition.x.ToString());
                row.Add(d.CoPposition.y.ToString());
                if (d.curColor == Target.posnIndicator.GREEN)
                {
                    row.Add("GREEN");
                }
                else if (d.curColor == Target.posnIndicator.YELLOW)
                {
                    row.Add("YELLOW");
                }
                else
                {
                    row.Add("RED");
                }
                row.Add(d.CoMposition.x.ToString());
                row.Add(d.CoMposition.y.ToString());
                row.Add(d.CoMposition.z.ToString());
                row.Add(d.targetNum.ToString());
                row.Add(d.trialNum.ToString());

                writer.WriteRow(row);
            }
        }

        Task.OnRecordContinuousData -= recordContinuousTrial;
    }

    /// <summary>
    /// A class that stores a line of the data collected during
    /// post-processing. This includes information such as the
    /// amount of time spent in each color 2 seconds before the
    /// player touched the target.
    /// </summary>
    class PostProcessingData
    {
        public readonly float precedingGreenTime;
        public readonly float precedingYellowTime;
        public readonly float precedingRedTime;

        public PostProcessingData(float precedingGreenTime,
            float precedingYellowTime, float precedingRedTime)
        {
            this.precedingGreenTime = precedingGreenTime;
            this.precedingYellowTime = precedingYellowTime;
            this.precedingRedTime = precedingRedTime;
        }
    }

    /// <summary>
    /// Calculates the list of post processing data to be written
    /// </summary>
    /// <returns></returns> The list of rows of postprocessing data to write
    private List<PostProcessingData> CalculatePostProcessingData()
    {
        List<PostProcessingData> result = new List<PostProcessingData>();

        // The time that the target was hit in the previous trial
        float prevTouchTime = 0f;

        foreach (Data d in data)
        {        
            // The time that the target was hit this trial
            float touchTime = d.time;
            
            itemGreenTime = 0f;
            itemYellowTime = 0f;
            itemRedTime = 0f;

            IncrementPrecedingTimes(touchTime, prevTouchTime);

            PostProcessingData resultItem = new PostProcessingData(itemGreenTime, itemYellowTime, itemRedTime);
            result.Add(resultItem);

            prevTouchTime = d.time;

        }

        return result;
    }

    /// <summary>
    /// Increments the preceding Color times to reflect how long 
    /// the player spent in each color during the preceding interval of this specific trial
    /// </summary>
    /// <param name="touchTime"></param> The time that the target was touched for this trial
    /// <param name="prevTouchTime"></param> The time that the target was touched for the previous trial
    private void IncrementPrecedingTimes(float touchTime, float prevTouchTime)
    {
        float previousFrameTime = 0f;

        foreach (ContinuousData d in continuousData)
        {
            // Look at the defined window preceding target touch
            if ((d.time <= touchTime) && (d.time > (touchTime - precedingInterval)) && (d.time > prevTouchTime))
            {
                if (d.curColor == Target.posnIndicator.GREEN)
                {
                    // Increment the green time by the difference in time between the previous frames
                    itemGreenTime = itemGreenTime + (d.time - previousFrameTime);
                }
                else if (d.curColor == Target.posnIndicator.YELLOW)
                {
                    itemYellowTime = itemYellowTime + (d.time - previousFrameTime);
                }
                else
                {
                    itemRedTime = itemRedTime + (d.time - previousFrameTime);
                }
            }

            previousFrameTime = d.time;
        }
    }

    /// <summary>
    /// Adds the post processing data for preceding color touching to the trial data
    /// </summary>
    /// <param name="ppData"></param> the post processing info to add to trial data
    private void AddPostProcessingToTrialData(List<PostProcessingData> ppData)
    {
        for (int i = 0; i < data.Count; i++)
        {
            // For each line of trial data, add the appropriate preceding color times
            PostProcessingData cur = ppData[i];
            data[i].AddPrecedingTimes(cur.precedingGreenTime, cur.precedingYellowTime, cur.precedingRedTime);
        }
    }
}
