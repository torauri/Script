using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class visualizeSpace_Script : MonoBehaviour {

	public Material hitMaterial;
	public Material nohitMaterial;
	private float delta_time;
	private float timedata;
	private StreamReader sr;
	private Vector3 pos;
	private int mode;
	private float pointer;

	//private bool hitflag;
	// Use this for initialization
	void Start () {
		this.GetComponent<Renderer>().material=nohitMaterial;
		delta_time = 0;
		mode = 1;
		pointer = 0;
		pos = transform.position;
		sr = new StreamReader(Application.dataPath + "/AnalysSpaceHip/" + transform.position.ToString() + ".csv");
		string strStream = sr.ReadToEnd();
		System.StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries;
		string [] lines = strStream.Split(new char[]{'\r', '\n' },option);
		if(lines.Length >0 ){
			timedata=float.Parse(lines[0]);
		}else{
			timedata = 100f;
		}
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKey("1")){
			mode = 1;
		}
		if(Input.GetKey("2")){
			mode = 2;
		}
		if(Input.GetKey("3")){
			mode = 3;
		}if(Input.GetKey("4")){
			mode = 4;
		}if(Input.GetKey("5")){
			mode = 5;
		}

		if(mode == 1){

			delta_time += 1f ;
			transform.position=new Vector3(transform.position.x,transform.position.y,transform.position.z-6.0f*Time.deltaTime);

			if(delta_time > 1500){
				delta_time = 0;
				this.GetComponent<Renderer>().material=nohitMaterial;
				transform.position = pos;
			}

			if(timedata + 50 >delta_time && delta_time>timedata){
				this.GetComponent<Renderer>().material=hitMaterial;
			}else{
				this.GetComponent<Renderer>().material=nohitMaterial;
			}
		}else if(mode == 2){
			if(10000>timedata){
				this.GetComponent<Renderer>().material=hitMaterial;
			}
		}else if(mode == 3){

			delta_time += 1f;
			transform.position=new Vector3(transform.position.x,transform.position.y,transform.position.z-5.0f);
			if(delta_time > 50){
				delta_time = 0;
				transform.position = pos;
			}
			if(40>timedata){
				this.GetComponent<Renderer>().material=hitMaterial;
			}
		}else if(mode == 4){
			delta_time = 0;
			this.GetComponent<Renderer>().material=nohitMaterial;
			transform.position = pos;
			if(100>timedata){
				this.GetComponent<Renderer>().material=hitMaterial;
			}
		}else if(mode == 5){

			if(Input.GetKey("right") && pointer<=55.0f){
				pointer += 1f;
			}else if(Input.GetKey("left") && pointer>=0){
				pointer -= 1f;
			}
			transform.position=new Vector3(transform.position.x,transform.position.y,pos.z-pointer);

			if(pos.z == pointer && 100>timedata){
				this.GetComponent<Renderer>().material=hitMaterial;
			}else{
				this.GetComponent<Renderer>().material=nohitMaterial;
			}

		}

	}
}
