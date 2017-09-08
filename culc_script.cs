using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class culc_script : MonoBehaviour {

	private StreamReader sr;
	private float[] minXlist = new float[51];
	private float[] maxXlist = new float[51];
	private float[] minYlist = new float[51];
	private float[] maxYlist = new float[51];

	// Use this for initialization
	void Start () {
		int pointer = 0;
		float minX,maxX,minY,maxY;
		for(float z=-1;z<=9;z=z+0.2f){//x座標
			pointer++;
			minX = 0;
			maxX = 0;
			minY = 0;
			maxY = 0;
			for(float y=0;y<=2;y=y+0.2f){//y座標
				for(float x=-1;x<=1;x=x+0.2f){//z座標
					Vector3 namepos = new Vector3(x,y,z);
					sr = new StreamReader(Application.dataPath + "/AnalysSpaceResultSLIDE/"+namepos.ToString()+".csv");
					string strStream = sr.ReadToEnd();
					System.StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries;
					string [] lines = strStream.Split(new char[]{'\r', '\n' },option);
					if(lines.Length >0 ){
						if(minX>=x) minX=x;
						if(minY>=y) minY=y;
						if(maxX<=x) maxX=x;
						if(maxY<=y) maxY=y;
					}
				}
			}
			minXlist[pointer]=minX;
			maxXlist[pointer]=maxX;
			minYlist[pointer]=minY;
			maxYlist[pointer]=maxY;
		}

		for(int i=1;i<minXlist.Length;i++){
			if(minXlist[i]-minXlist[i-1]>0.6 || minXlist[i]-minXlist[i-1]<-0.6){
				minXlist[i-1]=minXlist[i];
			}
		}

	}

}
