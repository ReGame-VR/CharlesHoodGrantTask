using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ReadWriteCSV;

public class TestWriter : MonoBehaviour {

    private List<string> data;

    private void WriteToFile()
    {
        using (CsvFileWriter writer = new CsvFileWriter("Data/WriteTest.csv"))
        {
            for (int i = 0; i < 100; i++)
            {
                CsvRow row = new CsvRow();
                for (int j = 0; j < 5; j++)
                    row.Add(string.Format("Column {0}", j));
                writer.WriteRow(row);
            }
        }
    }


	void OnDisable()
    {
        WriteToFile();
    }
    
    // Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
