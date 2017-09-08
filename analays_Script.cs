using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class analays_Script : MonoBehaviour {

	public Material hitMaterial;
	public Material nohitMaterial;
	private int analays_time = 2;
	private int delta_time;

	private StreamWriter sw;
	private FileInfo fi;

	//private bool hitflag;
	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer>().material=nohitMaterial;
		delta_time=0;


	}

	// Update is called once per frame
	void Update () {

		this.GetComponent<Rigidbody>().WakeUp();
		delta_time +=(int)(Time.deltaTime*1000f);

		if(delta_time >= analays_time*1000){
			GameObject.Destroy(gameObject);
		}
	}
	//接触している時
	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag=="analays_character"){

		 	this.GetComponent<Renderer>().material=hitMaterial;
			fi = new FileInfo(Application.dataPath + "/AnalysTimeResult/" + delta_time.ToString("0000") + ".csv");
			sw = fi.AppendText();

			sw.WriteLine(transform.position.ToString()+","+collider.gameObject.name);

			sw.Flush();
			sw.Close();
	 }
	}

	void OnTriggerStay(Collider collider){
		if(collider.gameObject.tag=="analays_character"){

			this.GetComponent<Renderer>().material=hitMaterial;
			fi = new FileInfo(Application.dataPath + "/AnalysTimeResult/" + delta_time.ToString("0000") + ".csv");
			sw = fi.AppendText();

			sw.WriteLine(transform.position.ToString() + "," + collider.gameObject.name);

			sw.Flush();
			sw.Close();
		}
	}

	void OnTriggerExit(Collider collider){
		if(collider.gameObject.tag=="analays_character"){

			this.GetComponent<Renderer>().material=nohitMaterial;
		}
	}

}
