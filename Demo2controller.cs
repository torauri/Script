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
	private float frameCount;
	private float targetFrame;
	private bool enemyFlag;
	private List<ComeObject> objectList = new List<ComeObject>();
	private List<Schedule> TimeLine = new List<Schedule>();
	private Schedule toFrame;


	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		frameCount = 0;
		enemyFlag = false;
		targetFrame = 0;

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
	void Update () {
		frameCount +=1f;
		Debug.Log(frameCount);
		if(TimeLine.Count > 0){
			if(toFrame.GetFrame() < frameCount){
				toFrame = TimeLine[0];
				TimeLine.RemoveAt(0);
			}
		}

		if(toFrame.GetStart() != "" && frameCount - toFrame.GetFrame()>=0 && frameCount - toFrame.GetFrame()<1){
			Debug.Log("play:"+toFrame.GetStart());
			anim.Play(toFrame.GetStart(),0);
		}

	}

	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag=="Enemy"){
			float[,] enemyArea = EnemyArea(collider.gameObject.transform.position.x,collider.gameObject.transform.position.y,3f,0.5f);
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
			enemyFlag = true;
			Demo2enemy enemy = collider.gameObject.GetComponent<Demo2enemy>();
			float enemyFrame = (collider.gameObject.transform.position.z - 0.15f - this.gameObject.transform.position.z)/enemy.speed;

			ComeObject obj = new ComeObject(JUMPdisList,SLIDEdisList,enemyFrame);
			objectList.Add(obj);

			if(objectList.Count > 0){
				scheduling(objectList[objectList.Count-1]);
			}else{
				//rescheduling
			}

		}
	}

	void scheduling(ComeObject obj){

		Schedule start = obj.ActTiming(JUMP,SLIDE,frameCount);
		TimeLine.Add(start);

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
	float frame = 0;
	float maxFrame = 0;
	float[,] position6 = new float[6,2];
	//Hip:0 LeftFoot:1 LeftHand:2 Head:3 RightHand:4 RightFoot:5

	public ActPosition2(string pass){
		string filename = System.IO.Path.GetFileNameWithoutExtension(pass);
		name = filename.Substring(0,filename.Length-2);
		frame = float.Parse(filename.Substring(filename.Length-2));
		if(name == "JUMP"){
			maxFrame = 56;
		}else{
			maxFrame = 40;
		}

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

	public float GetFrame(){
		return frame;
	}

	public float GetPoint(){
		return frame/maxFrame;
	}

	public void DebugSphere(){
		for(int i=0;i<6;i++){
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			sphere.transform.position = new Vector3 (position6[i,0], position6[i,1], 0);
			sphere.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		}
	}


	public float AreaChecker(float[,] enemy){
		float r = 1f;
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

}

public class ComeObject{
	float[] JUMPdisList ;
	float[] SLIDEdisList;
	float comeTiming;

	public ComeObject(float[] j,float[] s,float t){
		JUMPdisList = new float[j.Length];
		SLIDEdisList = new float[s.Length];

		j.CopyTo(JUMPdisList,0);
		s.CopyTo(SLIDEdisList,0);
		comeTiming = t;
		Debug.Log(comeTiming);
	}

	public float GetTiming(){
		return comeTiming;
	}

	public Schedule ActTiming(List<ActPosition2> j,List<ActPosition2> s,float f){
		int maxJ = 0;
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
			return new Schedule(f+comeTiming-j[maxJ].GetFrame(),j[0]);
		}else{
			Debug.Log("SLIDE!");
			return new Schedule(f+comeTiming-s[maxS].GetFrame(),s[0]);
		}
	}
}

public class Schedule{
	float frame;
	ActPosition2 act;
	string StartAction = "";
	public Schedule(float f,ActPosition2 a){
		frame = f;
		act = a;
		Debug.Log(a.GetName()+frame.ToString());
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

}
