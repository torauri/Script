using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class visualize_line : MonoBehaviour {

	private StreamReader sr;
	public string name;

	// Use this for initialization
	void Start () {
		sr = new StreamReader(Application.dataPath + "/AnalysSpaceLine/" + name + ".csv");
		string strStream = sr.ReadToEnd();

		System.StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries;
		string [] lines = strStream.Split(new char[]{'\r', '\n' },option);
		char [] spliter = new char [1]{','};
		float[] xdata=new float[lines.Length];
		float[] ydata=new float[lines.Length];
		for(int i=0;i<lines.Length;i++){
			string[] splitedData = lines [i].Split(spliter,option);
			xdata[i] = float.Parse(splitedData[0]);
			ydata[i] = float.Parse(splitedData[1]);
		}

		LineRenderer lineRenderer = this.GetComponent<LineRenderer>();
		lineRenderer.enabled = true;
		lineRenderer.SetVertexCount(xdata.Length);
		for(int i=0;i<xdata.Length;i++){
			Vector3 pos = new Vector3(xdata[i],ydata[i],(float)i*0.5f);
			lineRenderer.SetPosition(i,pos);
		}

	}

}
