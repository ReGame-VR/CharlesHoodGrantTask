using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestWriter : MonoBehaviour {

    private List<string> data;

    private void WriteToFile()
    {
        using (StreamWriter file = new StreamWriter(@"C:\Users\Rehabilitation Games\Desktop\CharlesHoodGrantTask\Assets\Data\test.txt"))
        {
            file.Write("Writing to file 2");
            file.Close();
        }
    }

	void OnDisable()
    {
        WriteToFile();
    }
    
    // Use this for initialization
	void Start () {
        using (StreamWriter file = new StreamWriter(@"C:\Users\Rehabilitation Games\Desktop\CharlesHoodGrantTask\Assets\Data\test.txt"))
        file.Write("Writing to file");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
