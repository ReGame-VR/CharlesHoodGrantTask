﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;

/// <summary>
/// Writes a line of data after every trial, giving information on the trial.
/// </summary>
public class DataHandler : MonoBehaviour {

    // stores the data for writing to file at end of task
    List<Data> data = new List<Data>();

	/// <summary>
    /// 
    /// </summary>
	void Awake () {
		// TODO: Add listener for recording data
	}

    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        // TODO: remove listener for recording data

        // Write all entries in data list to file
        using (CsvFileWriter writer = new CsvFileWriter(@"Data/test.csv"))
        {
            CsvRow header = new CsvRow();
            header.Add("Trial Number");
            header.Add("Time");
            header.Add("Target Number");
            header.Add("Weight Shift Success");
            header.Add("Button Success");
            header.Add("Random Sequence?");
            header.Add("X Coord of COP");
            header.Add("Y Coord of COP");
            header.Add("COP Total Path");
            header.Add("Target Score");
            header.Add("Trial Score");
            header.Add("Cumulative Score");

            foreach (Data d in data)
            {
                CsvRow row = new CsvRow();

                row.Add(d.trialNum.ToString());
                row.Add(d.time.ToString());
                row.Add(d.targetNum.ToString());
                row.Add(d.targetTime.ToString());
                row.Add(d.weightShiftSuccess.ToString());
                row.Add(d.buttonSuccess.ToString());
                row.Add(d.isRandomSequence.ToString());
                row.Add(d.weightPosn.x.ToString());
                row.Add(d.weightPosn.y.ToString());
                row.Add(d.COPTotalPath.ToString());
                row.Add(d.targetScore.ToString());
                row.Add(d.trialScore.ToString());
                row.Add(d.cumulativeScore.ToString());
            }
        }
    }
	
    /// <summary>
    /// A class that stores info on each trial relevant to data recording. Every field is
    /// public readonly, so can always be accessed, but can only be assigned once in the
    /// constructor.
    /// </summary>
    class Data
    {
        public readonly int trialNum; // which trial it is
        public readonly float time; 
        public readonly int targetNum; // the index of the randomly selected target, ONE INDEXED
        public readonly float targetTime; // time to activate target - 10 if unsuccessful
        public readonly bool weightShiftSuccess; // did they shift to correct location?
        public readonly bool buttonSuccess; // did they activate target while in correct location?
        public readonly bool isRandomSequence; // are the targets selected randomly or from a set sequence
        public readonly Vector2 weightPosn; // weight posn on balance board 
                                            // - when target hit OR when time runs out
        public readonly float COPTotalPath; // The total distance the user's COB traveled during the task
        public readonly int targetScore; // the combined accuracy + timeliness score for a single trial
        public readonly int trialScore; // the trial score, so far (a set of 5 target positions)
        public readonly int cumulativeScore; // The total score for the session

        /// <summary>
        /// Constructs an instance of the Data class.
        /// </summary>
        /// <param name="trialNum"></param> The trial #
        /// <param name="time"></param> ???
        /// <param name="targetNum"></param> the index of the target, ONE INDEXED
        /// <param name="targetTime"></param> time to activate target or 10 if not activated
        /// <param name="weightShiftSuccess"></param> did they successfully weight shift to correct location
        /// <param name="buttonSuccess"></param> did they activate the target while in the correct location?
        /// <param name="isRandomSequence"></param> is the target a random selection or a set sequence
        /// <param name="weightPosn"></param> the 2D posn of COB when target activated OR when time up
        /// <param name="COPTotalPath"></param> the distance in CM that the COB shifted during the target
        /// <param name="targetScore"></param> the score earned just from this target
        /// <param name="trialScore"></param> the score earned just in this trial (set of 5 targets)
        /// <param name="cumulativeScore"></param> the total score so far at the time data is submitted
        public Data(int trialNum, float time, int targetNum, float targetTime, 
            bool weightShiftSuccess, bool buttonSuccess, bool isRandomSequence, 
            Vector2 weightPosn, float COPTotalPath, int targetScore, int trialScore,
            int cumulativeScore)
        {
            this.trialNum = trialNum;
            this.time = time;
            this.targetNum = targetNum;
            this.targetTime = targetTime;
            this.weightShiftSuccess = weightShiftSuccess;
            this.buttonSuccess = buttonSuccess;
            this.isRandomSequence = isRandomSequence;
            this.weightPosn = weightPosn;
            this.COPTotalPath = COPTotalPath;
            this.targetScore = targetScore;
            this.trialScore = trialScore;
            this.cumulativeScore = cumulativeScore;
        }
    }
}
