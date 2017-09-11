using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Demo2controller : MonoBehaviour {

	private Animator anim;
	private StreamReader sr;
	private List<ActPosition2> JUMP = new List<ActPosition2>();
	private List<ActPosition2> SLIDE = new List<ActPosition2>();
	private ActPosition2 RUN;
	private int frameCount;
	private List<ComeObject> objectList = new List<ComeObject>();
	private List<Schedule> TimeLine = new List<Schedule>();


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		frameCount = 0;


		string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Analys7Position/","*",System.IO.SearchOption.TopDirectoryOnly);

		foreach(string s in files){
			if(s.IndexOf(".meta")<0){
				ActPosition2 pos = new ActPosition2(s);
				switch(pos.GetName()){
					case "JUMP":
					JUMP.Add(pos);
					break;

					case "SLIDE":
					SLIDE.Add(pos);
					break;

				}
			}
		}
		anim.Play("RUN",0);
	}

	// Update is called once per frame
	void FixedUpdate () {
		frameCount +=1;
		//Debug.Log("Frame:"+frameCount.ToString());

		if(TimeLine.Count > 0){
			 if(frameCount - TimeLine[0].GetStart()==0 ){
				Debug.Log("play:"+TimeLine[0].GetAction());
				anim.Play(TimeLine[0].GetAction(),0);
			 }

			if(TimeLine[0].GetEnd() <= frameCount){
				TimeLine.RemoveAt(0);
			}
		}

	}

	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag=="Enemy"){
			float[,] enemyArea = EnemyArea(collider.gameObject.transform.position.x,collider.gameObject.transform.position.y,2f,0.3f);
			float[] JUMPdisList = new float[JUMP.Count];
			int i=0;
			foreach(ActPosition2 a in JUMP){
				JUMPdisList[i]=a.AreaChecker(enemyArea);
				i++;
			}

			float[] SLIDEdisList = new float[SLIDE.Count];
			i=0;
			foreach(ActPosition2 a in SLIDE){
				SLIDEdisList[i]=a.AreaChecker(enemyArea);
				i++;
			}

			Demo2enemy enemy = collider.gameObject.GetComponent<Demo2enemy>();
			int enemyFrame = (int)((collider.gameObject.transform.position.z - 0.15f - this.gameObject.transform.position.z)/enemy.speed);

			ComeObject obj = new ComeObject(JUMPdisList,SLIDEdisList,enemyFrame,frameCount);
			objectList.Add(obj);

			if(TimeLine.Count == 0){
				Scheduling(objectList[objectList.Count-1]);
			}else{
				ReScheduling(objectList[objectList.Count-1]);
			}

		}
	}

	void Scheduling(ComeObject obj){

		Schedule start = obj.ActTiming(JUMP,SLIDE,frameCount);
		TimeLine.Add(start);

	}

	void ReScheduling(ComeObject obj){
		Schedule start = obj.ActTiming(JUMP,SLIDE,frameCount);
		if(start.GetStart()>TimeLine[TimeLine.Count-1].GetEnd()){
			TimeLine.Add(start);
		}else{
			Debug.Log("matchng");
			int timingDis = obj.GetFrame() - objectList[objectList.Count-2].GetFrame();
			if(obj.JUMPTime()>=timingDis && objectList[objectList.Count-2].JUMPTime()>=timingDis){
				Debug.Log("ALLJUMP");
				int AllJUMPframe = 0;
				for(int i=0;i<JUMP.Count-timingDis/2;i++){
					if(objectList[objectList.Count-2].GetDis("JUMP",i)>0 && obj.GetDis("JUMP",i+timingDis/2) > 0){
						AllJUMPframe = i*2;
						break;
					}
				}
				TimeLine[TimeLine.Count-1].ReSet("JUMP",objectList[objectList.Count-2].GetFrame()-AllJUMPframe);
				return;

			}else if(obj.SLIDETime()>=timingDis && objectList[objectList.Count-2].SLIDETime()>=timingDis){
				Debug.Log("AllSlide");
			}
		}
	}

	float[,] EnemyArea(float xpos,float ypos,float xlen,float ylen){
		float[,] result = new float[4,2];
		result[0,0]=xpos-xlen/2f;
		result[0,1]=ypos+ylen/2f;

		result[1,0]=xpos+xlen/2f;
		result[1,1]=ypos+ylen/2f;

		result[2,0]=xpos+xlen/2f;
		result[2,1]=ypos-ylen/2f;

		result[3,0]=xpos-xlen/2f;
		result[3,1]=ypos-ylen/2f;

		return result;
	}

	int maxIndex(float[] x){
		int maxI = 0;
		for(int i=0;i<x.Length;i++){
			if(x[i]>x[maxI]){
				maxI = i;
			}

		}
		return maxI;
	}

}


public class ActPosition2{
	string name = "";
	int frame = 0;

	float[,] position7 = new float[7,2];
	//Hip:0 LeftFoot:1 LeftHand:2 Head:3 RightHand:4 RightFoot:5

	public ActPosition2(string pass){
		string filename = System.IO.Path.GetFileNameWithoutExtension(pass);
		name = filename.Substring(0,filename.Length-2);
		frame = int.Parse(filename.Substring(filename.Length-2));

		StreamReader sr = new StreamReader(pass);
		string strStream = sr.ReadToEnd();
		System.StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries;
		string [] lines = strStream.Split(new char[]{'\r', '\n' },option);
		char [] spliter = new char [1]{','};
		for(int i=0;i<lines.Length;i++){
			string[] splitedData = lines [i].Split(spliter,option);

			switch(splitedData[0]){
				case "Head":
				position7[3,0]=float.Parse(splitedData[1]);
				position7[3,1]=float.Parse(splitedData[2]);
				break;

				case "Hip":
				position7[0,0]=float.Parse(splitedData[1]);
				position7[0,1]=float.Parse(splitedData[2]);
				break;

				case "LeftHand":
				position7[2,0]=float.Parse(splitedData[1]);
				position7[2,1]=float.Parse(splitedData[2]);
				break;

				case "RightHand":
				position7[4,0]=float.Parse(splitedData[1]);
				position7[4,1]=float.Parse(splitedData[2]);
				break;

				case "LeftFoot":
				position7[1,0]=float.Parse(splitedData[1]);
				position7[1,1]=float.Parse(splitedData[2]);
				break;

				case "RightFoot":
				position7[5,0]=float.Parse(splitedData[1]);
				position7[5,1]=float.Parse(splitedData[2]);
				break;

				case "Body":
				position7[6,0]=float.Parse(splitedData[1]);
				position7[6,1]=float.Parse(splitedData[2]);
				break;
			}
		}
	}

	public string GetName(){
		return name;
	}

	public int GetFrame(){
		return frame;
	}

	public float AreaChecker(float[,] enemy){

		float r = 0.15f;
		for(int i=0;i<7;i++){
			Vector2[] v = new Vector2[4];
			Vector2[] m = new Vector2[4];
			bool flag = true;
			for(int j=0;j<4;j++){
				v[j] = new Vector2(enemy[(j+1)%4,0]-enemy[j,0],enemy[(j+1)%4,1]-enemy[j,1]);
				m[j] = new Vector2(position7[i,0]-enemy[j,0],position7[i,1]-enemy[j,1]);
			}
			for(int j=0;j<4;j++){
				if(Vector2.Dot(v[j],m[j])>=0 && Vector2.Dot(v[j],m[(j+1)%4])<=0 && System.Math.Abs(Cross(v[j],m[j]))/v[j].magnitude <= r){
					return 0;
				}
				if(square(position7[i,0]-enemy[j,0]) + square(position7[i,1]-enemy[j,1]) <= square(r) || square(position7[i,0]-enemy[(j+1)%4,0]) + square(position7[i,1]-enemy[(j+1)%4,1]) <= square(r)){
					return 0;
				}
				if(Cross(v[j],m[j])<0){
					flag = false;
				}
			}
			if(flag == true){
				return 0;
			}
		}
		float dis = 0;
		float posx = (enemy[0,0]+enemy[2,0])/2f;
		float posy = (enemy[0,1]+enemy[2,1])/2f;
		for(int i=0;i<5;i++){
			dis += new Vector2(position7[i,0]-posx,position7[i,1]-posy).magnitude;
		}
		return dis;

	}

	float Cross(Vector2 a,Vector2 b){
		return (float)(a.x*b.y - b.x*a.y);
	}
	float square(float x){
		return x*x;
	}

	public void DebugSphere(){
		for(int i=0;i<6;i++){
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = new Vector3 (position7[i,0], position7[i,1], 0);
			sphere.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		}
	}

}

public class ComeObject{
	float[] JUMPdisList ;
	float[] SLIDEdisList;
	int comeTiming;
	int comeFrame;

	public ComeObject(float[] j,float[] s,int t,int f){
		JUMPdisList = new float[j.Length];
		SLIDEdisList = new float[s.Length];

		j.CopyTo(JUMPdisList,0);
		s.CopyTo(SLIDEdisList,0);
		comeTiming = t;
		comeFrame = f+t;
		Debug.Log(comeTiming);
	}

	public int GetTiming(){
		return comeTiming;
	}

	public int GetFrame(){
		return comeFrame;
	}

	public float GetDis(string name,int i){
		if(name == "JUMP"){
			return JUMPdisList[i];
		}else if(name == "SLIDE"){
			return SLIDEdisList[i];
		}
		return 0;
	}

	public int JUMPTime(){
		int t=0;
		for(int i=0;i<JUMPdisList.Length;i++){
			if(JUMPdisList[i]>0){
				t++;
			}
		}
		return t*2;
	}

	public int SLIDETime(){
		int t=0;
		for(int i=0;i<SLIDEdisList.Length;i++){
			if(SLIDEdisList[i]>0){
				t++;
			}
		}
		return t*2;
	}

	public Schedule ActTiming(List<ActPosition2> j,List<ActPosition2> s,int f){
		int maxJ = 0;
		Schedule result;
		for(int i=0;i<JUMPdisList.Length;i++){
			if(JUMPdisList[i]>JUMPdisList[maxJ]){
				maxJ = i;
			}
		}

		int maxS = 0;
		for(int i=0;i<SLIDEdisList.Length;i++){
			if(SLIDEdisList[i]>SLIDEdisList[maxS]){
				maxS = i;
			}
		}

		if(JUMPdisList[maxJ]>=SLIDEdisList[maxS]){
			Debug.Log("JUMP!");
			result = new Schedule(f+comeTiming-j[maxJ].GetFrame(),56,"JUMP");
			return result;
		}else{
			Debug.Log("SLIDE!");
			result = new Schedule(f+comeTiming-s[maxS].GetFrame(),40,"SLIDE");
			return result;
		}
	}
}

public class Schedule{
	int startFrame;
	int length;
	string action = "";
	public Schedule(int f,int l,string act){
		startFrame = f;
		length = l;
		action = act;
		Debug.Log("start"+startFrame);
	}

	public int GetStart(){
		return startFrame;
	}

	public void ReSet(string s,int f){
		startFrame = f;
		action = s;
		Debug.Log("reset" + f);
		if(s == "JUMP"){
			length=56;
		}else{
			length=40;
		}
	}

	public int GetLength(){
		return length;
	}

	public int GetEnd(){
		return startFrame + length;
	}

	public string GetAction(){
		return action;
	}

}
