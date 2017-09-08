using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class analysSpace_script : MonoBehaviour {

	public Material hitMaterial;
	public Material nohitMaterial;
	//private float analays_time = 1.5f;
	private int framecount;
	private StreamWriter sw;
	private FileInfo fi;
	private bool startflag = false;

	//private bool hitflag;
	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer>().material=nohitMaterial;

		fi = new FileInfo(Application.dataPath + "/AnalysSpaceHip/" + transform.position.ToString() + ".csv");
		fi.Delete();

		sw = fi.AppendText();

		framecount=0;
		startflag = true;
	}

	// Update is called once per frame
	void Update () {

		//this.GetComponent<Rigidbody>().WakeUp();
		framecount++;
		transform.position=new Vector3(transform.position.x,transform.position.y,transform.position.z-0.5f);

		if(framecount > 55){
			sw.Flush();
			sw.Close();

			GameObject.Destroy(gameObject);
		}
	}
	//接触している時
	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag=="analays_character" && startflag){
			this.GetComponent<Renderer>().material=hitMaterial;
		 	sw.WriteLine(framecount.ToString());
	 }
	}

	/*void OnTriggerStay(Collider collider){
		if(collider.gameObject.tag=="analays_character" ){

			this.GetComponent<Renderer>().material=hitMaterial;

		}
	}*/
/*
	void OnTriggerExit(Collider collider){
		if(collider.gameObject.tag=="analays_character"){

			this.GetComponent<Renderer>().material=nohitMaterial;
			hit = "";

		}
	}*/

}
