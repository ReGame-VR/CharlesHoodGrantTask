using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadWriteCSV;

public class DataHandler : MonoBehaviour {

    List<Data> data = new List<Data>();



	// Use this for initialization
	void Awake () {
		// TODO: Add listener for recording data
	}

    void OnDisable()
    {
        // TODO: remove listener for recording data

        using (CsvFileWriter writer = new CsvFileWriter(@"Data/test.csv"))
        {
            CsvRow header = CsvRow();
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
	
	// Update is called once per frame
	void Update () {
		
	}

    class Data
    {
        public readonly int trialNum;
        public readonly float time; // ask d. levac for clarification
        public readonly int targetNum; // the index of the randomly selected target?
        public readonly float targetTime; // time to activate target - 10 if unsuccessful
        public readonly bool weightShiftSuccess; // did they shift to correct location?
        public readonly bool buttonSuccess; // did they activate target while in correct location?
        public readonly bool isRandomSequence;
        public readonly Vector2 weightPosn; // weight posn on balance board - when target hit OR when time runs out
        public readonly float COPTotalPath; // how to calculate?
        public readonly int targetScore;
        public readonly int trialScore;
        public readonly int cumulativeScore;

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
