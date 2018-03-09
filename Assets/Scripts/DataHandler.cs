using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;

/// <summary>
/// Writes a line of data after every trial, giving information on the trial.
/// </summary>
public class DataHandler : MonoBehaviour {

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
        WriteTrialFile();
        WriteContinuousFile();
    }

    /// <summary>
    /// Creates a new instance of the data class to store information to write to the data file 
    /// at the end of the session. It is called when the trial over event fires.
    /// </summary>
    /// <param name="trialNum"> Which trial number this is </param>
    /// <param name="time"> global time
    /// <param name="targetNum"> the index of the target - one indexed for recording purposes</param>
    /// <param name="targetTime"> Time it took to weight shift and touch target, or 10 seconds
    ///                           if target was not successfully touched </param>
    /// <param name="weightShiftSuccess"> did the user have weight shift success? </param>
    /// <param name="buttonSuccess"> did the user successfully touch the target? </param>
    /// <param name="isRandomSequence"> true if the trial is the set sequence, not random </param>
    /// <param name="weightPosn"> the user's COB position at the end of the trial </param>
    /// <param name="COPTotalPath"> the total distance, in cm, that the user's COB traveled </param>
    /// <param name="targetScore"> score earned for this target only </param>
    /// <param name="trialScore"> score earned for this trial so far </param>
    /// <param name="cumulativeScore"> overall score earned so far </param>
    private void recordTrial(string participantId, bool rightHanded, bool isRotation, int trialNum, 
            float time, int targetNum, float targetTime,
            bool weightShiftSuccess, bool buttonSuccess, bool isRandomSequence,
            Vector2 weightPosn, float COPTotalPath, float trialScore,
            float cumulativeScore)
    {
        data.Add(new Data(participantId, rightHanded, isRotation, trialNum, time,
            targetNum, targetTime, weightShiftSuccess, buttonSuccess,
            isRandomSequence, weightPosn, COPTotalPath, trialScore, cumulativeScore));
    }

    // Records the continuous data like CoP and CoM to the list of data
    private void recordContinuousTrial(string participantId, float time, Vector2 CoPposition, bool weightShiftSuccess,
            Vector3 CoMposition, int targetNum, int trialNum)
    {
        continuousData.Add(new ContinuousData(participantId, time, CoPposition, weightShiftSuccess, CoMposition, targetNum, trialNum));
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
        public readonly float trialScore; // the trial score, so far (a set of 5 target positions)
        public readonly float cumulativeScore; // The total score for the session

        /// <summary>
        /// Constructs an instance of the Data class.
        /// </summary>
        /// <param name="trialNum"> The trial number </param>
        /// <param name="time"> global time </param>
        /// <param name="targetNum"> the index of the target, ONE INDEXED </param>
        /// <param name="targetTime"> ime to activate target or 10 if not activated </param>
        /// <param name="weightShiftSuccess"> did they successfully weight shift to correct location </param>
        /// <param name="buttonSuccess"> did they activate the target while in the correct location? </param>
        /// <param name="isRandomSequence"> is the target a random selection or a set sequence </param>
        /// <param name="weightPosn"> the 2D posn of COB when target activated OR when time up </param>
        /// <param name="COPTotalPath"> the distance in CM that the COB shifted during the target </param>
        /// <param name="targetScore"> the score earned just from this target </param>
        /// <param name="trialScore"> the score earned just in this trial (set of 5 targets) </param>
        /// <param name="cumulativeScore"> the total score so far at the time data is submitted </param>
        public Data(string participantId, bool rightHanded, bool isRotation, int trialNum, 
            float time, int targetNum, float targetTime, 
            bool weightShiftSuccess, bool buttonSuccess, bool isRandomSequence, 
            Vector2 weightPosn, float COPTotalPath, float trialScore,
            float cumulativeScore)
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
        public readonly bool weightShiftSuccess; // is the CoP in the CoP target area?
        public readonly Vector3 CoMposition; // current CoM position
        public readonly int targetNum; // current target number (one-indexed)
        public readonly int trialNum; // current trial number

        public ContinuousData(string participantId, float time, Vector2 CoPposition, bool weightShiftSuccess,
            Vector3 CoMposition, int targetNum, int trialNum)
        {
            this.participantId = participantId;
            this.time = time;
            this.CoPposition = CoPposition;
            this.weightShiftSuccess = weightShiftSuccess;
            this.CoMposition = CoMposition;
            this.targetNum = targetNum;
            this.trialNum = trialNum;
        }
    }

    /// <summary>
    /// Writes the Trial File to a CSV
    /// </summary>
    private void WriteTrialFile()
    {
        // Write all entries in data list to file
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/DataParticipantID" + GlobalControl.Instance.participantID + ".csv"))
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
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/ContinuousCoPdata" + GlobalControl.Instance.participantID + ".csv"))
        {
            Debug.Log("Writing continuous data to file");
            // write header
            CsvRow header = new CsvRow();
            header.Add("Participant ID");
            header.Add("Global Time");
            header.Add("CoP X");
            header.Add("CoP Y");
            header.Add("Weight Shift Success?");
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
                if (d.weightShiftSuccess)
                {
                    row.Add("YES");
                }
                else
                {
                    row.Add("NO");
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
}
