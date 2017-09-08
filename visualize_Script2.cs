using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class visualize_Script2 : MonoBehaviour {

	private int delta_time;
	private int pointer;
	private int[] timedata;
	private List<string> filelist = new List<string>();
	private StreamReader sr;
	private Vector3[] positions;
	public GameObject sphere;

	//private bool hitflag;
	// Use this for initialization
	void Start () {
		delta_time=0;
		pointer = 0;

		//全ファイルパスの読み取り
		string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/AnalysTimeResult/","*",System.IO.SearchOption.TopDirectoryOnly);

		foreach(string s in files){
			if(s.IndexOf(".meta")<0){
				filelist.Add(s);
			}
		}


		timedata = new int[filelist.Count];

		for(int i=0;i<filelist.Count;i++){
			timedata[i] = int.Parse(System.IO.Path.GetFileNameWithoutExtension(filelist[i]));
			Debug.Log(i);
			Debug.Log(timedata[i]);
		}

	}

	// Update is called once per frame
	void Update () {

		delta_time +=(int)(Time.deltaTime*1000f);

			if(delta_time > timedata[timedata.Length-1]){
				delta_time = 0;
				pointer=0;
			}

			if(delta_time > timedata[pointer] || delta_time==0){
				foreach(Transform n in gameObject.transform){
					GameObject.Destroy(n.gameObject);
				}
				pointer++;
				sr = new StreamReader(filelist[pointer]);
				string strStream = sr.ReadToEnd();
				System.StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries;
				string [] lines = strStream.Split(new char[]{'\r', '\n' },option);
				positions = new Vector3[lines.Length];
				char [] spliter = new char [1]{','};

				for(int i=0;i<lines.Length;i++){
					string[] splitedData = lines [i].Substring(1,lines[i].Length-2).Split(spliter,option);
					positions[i]=new Vector3(float.Parse(splitedData[0]),float.Parse(splitedData[1]),float.Parse(splitedData[2]));
				}
				foreach(Vector3 v in positions){
					GameObject obj = Instantiate(sphere,v,transform.rotation);
					obj.transform.SetParent(transform);
				}

		}
	}
}
