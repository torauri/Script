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
	private Schedule toFrame;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		frameCount = 0;


		string[] files = System.IO.Directory.GetFiles(Application.dataPath + "/Analys6Position/","*",System.IO.SearchOption.TopDirectoryOnly);

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

					case "RUN":
					RUN = pos;
					break;
				}
			}
		}
		toFrame = new Schedule(0,RUN);
		anim.Play("RUN",0);
	}

	// Update is called once per frame
	void FixedUpdate () {
		frameCount +=1;
		Debug.Log("Frame:"+frameCount.ToString() +" TimeLineCount:"+TimeLine.Count.ToString());


		if(toFrame.GetStart() != "" && frameCount - toFrame.GetFrame()==0){
			Debug.Log("play:"+toFrame.GetStart());
			anim.Play(toFrame.GetStart(),0);
		}
		if(TimeLine.Count > 0){
			if(toFrame.GetFrame() < frameCount){
				toFrame = TimeLine[0];
				TimeLine.RemoveAt(0);
			}
		}

	}

	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag=="Enemy"){
			float[,] enemyArea = EnemyArea(collider.gameObject.transform.position.x,collider.gameObject.transform.position.y,3f,1f);
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

			ComeObject obj = new ComeObject(JUMPdisList,SLIDEdisList,enemyFrame);
			objectList.Add(obj);

			if(TimeLine.Count == 0){
				Scheduling(objectList[objectList.Count-1]);
			}else{
				ReScheduling(objectList[objectList.Count-1]);
			}

		}
	}

	void Scheduling(ComeObject obj){

		List<Schedule> start = obj.ActTiming(JUMP,SLIDE,frameCount);
		TimeLine.AddRange(start);

	}

	void ReScheduling(ComeObject obj){
		List<Schedule> start = obj.ActTiming(JUMP,SLIDE,frameCount);
		if(start[0].GetFrame()>TimeLine[TimeLine.Count-1].GetFrame()){
			TimeLine.AddRange(start);
		}else{
			 ActPosition2 afterAct = TimeLine[obj.GetTiming()].GetAct();
			 afterAct.DebugSphere();
			 Debug.Log("afterAct:"+afterAct.GetName()+afterAct.GetFrame().ToString());
			 float afterDis;
			 afterDis = obj.GetDis(afterAct.GetName(),afterAct.GetFrame()/2);
			 Debug.Log(afterDis);
			 if(afterDis >0){
				 Debug.Log("NoChange");
				 return;
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

	float[,] position6 = new float[6,2];
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
				position6[3,0]=float.Parse(splitedData[1]);
				position6[3,1]=float.Parse(splitedData[2]);
				break;

				case "Hip":
				position6[0,0]=float.Parse(splitedData[1]);
				position6[0,1]=float.Parse(splitedData[2]);
				break;

				case "LeftHand":
				position6[2,0]=float.Parse(splitedData[1]);
				position6[2,1]=float.Parse(splitedData[2]);
				break;

				case "RightHand":
				position6[4,0]=float.Parse(splitedData[1]);
				position6[4,1]=float.Parse(splitedData[2]);
				break;

				case "LeftFoot":
				position6[1,0]=float.Parse(splitedData[1]);
				position6[1,1]=float.Parse(splitedData[2]);
				break;

				case "RightFoot":
				position6[5,0]=float.Parse(splitedData[1]);
				position6[5,1]=float.Parse(splitedData[2]);
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

		float r = 1.2f;
		for(int i=0;i<5;i++){
			Vector2[] v = new Vector2[4];
			Vector2[] m = new Vector2[4];
			bool flag = true;
			for(int j=0;j<4;j++){
				v[j] = new Vector2(enemy[(j+1)%4,0]-enemy[j%4,0],enemy[(j+1)%4,1]-enemy[j%4,1]);
				m[j] = new Vector2(position6[i,0]-enemy[j%4,0],position6[i,1]-enemy[j%4,1]);
			}
			for(int j=0;j<4;j++){
				if(Vector2.Dot(v[j],m[j])<=0 && Vector2.Dot(v[j],m[(j+1)%4])>=0 && System.Math.Abs(gaiseki(v[j],m[j]))/v[j].magnitude <= r){
					return 0;
				}
				if(square(position6[i,0]-enemy[j,0]) + square(position6[i,1]-enemy[j,1]) <= square(r) || square(position6[i,0]-enemy[(j+1)%4,0]) + square(position6[i,1]-enemy[(j+1)%4,1]) <= square(r)){
					return 0;
				}
				if(gaiseki(v[j],m[j])>0){
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
			dis += new Vector2(position6[i,0]-posx,position6[i,1]-posy).magnitude;
		}
		return dis;

	}

	float gaiseki(Vector2 a,Vector2 b){
		return (float)(a.x*b.y - b.x*a.y);
	}
	float square(float x){
		return x*x;
	}

	public void DebugSphere(){
		for(int i=0;i<6;i++){
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = new Vector3 (position6[i,0], position6[i,1], 0);
			sphere.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		}
	}

}

public class ComeObject{
	float[] JUMPdisList ;
	float[] SLIDEdisList;
	int comeTiming;

	public ComeObject(float[] j,float[] s,int t){
		JUMPdisList = new float[j.Length];
		SLIDEdisList = new float[s.Length];

		j.CopyTo(JUMPdisList,0);
		s.CopyTo(SLIDEdisList,0);
		comeTiming = t;
		Debug.Log(comeTiming);
	}

	public int GetTiming(){
		return comeTiming;
	}

	public float GetDis(string name,int i){
		if(name == "JUMP"){
			return JUMPdisList[i];
		}else if(name == "SLIDE"){
			return SLIDEdisList[i];
		}
		return 0;
	}

	public List<Schedule> ActTiming(List<ActPosition2> j,List<ActPosition2> s,int f){
		int maxJ = 0;
		List<Schedule> result = new List<Schedule>();
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
			for(int i=0;i<j.Count;i++){
				result.Add(new Schedule(f+comeTiming-j[maxJ].GetFrame()+(i*2),j[i]));
				result.Add(new Schedule(f+comeTiming-j[maxJ].GetFrame()+(i*2)+1,j[i]));
			}
			return result;
		}else{
			Debug.Log("SLIDE!");
			for(int i=0;i<s.Count;i++){
				result.Add(new Schedule(f+comeTiming-s[maxS].GetFrame()+(i*2),s[i]));
				result.Add(new Schedule(f+comeTiming-j[maxJ].GetFrame()+(i*2)+1,j[i]));
			}
			return result;
		}
	}
}

public class Schedule{
	int frame;
	ActPosition2 act;
	string StartAction = "";
	public Schedule(int f,ActPosition2 a){
		frame = f;
		act = a;
		if(act.GetFrame() == 0){
			StartAction = act.GetName();
		}
	}

	public string GetStart(){
		return StartAction;
	}

	public float GetFrame(){
		return frame;
	}

	public ActPosition2 GetAct(){
		return act;
	}

}
