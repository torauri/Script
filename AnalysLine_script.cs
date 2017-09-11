using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class AnalysLine_script : MonoBehaviour {

	private StreamWriter sw;
	private FileInfo fi;
	private int framecount;
	public string positionName;
	private string motionName = "JUMP";

	// Use this for initialization
	void Start () {

		framecount = 0;
	}

	// Update is called once per frame
	void Update () {
		fi = new FileInfo(Application.dataPath + "/Analys7Position/"+motionName+ framecount.ToString("00") +".csv");
		sw = fi.AppendText();

		sw.WriteLine(positionName+","+transform.position.x.ToString() + "," + transform.position.y.ToString());

		sw.Flush();
		sw.Close();

		framecount += 2;

	}
}
