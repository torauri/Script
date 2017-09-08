using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animcontroler : MonoBehaviour {
	private float interval;
	private Animator anim;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		anim.SetBool("jump",true);
		interval = 2f;
	}

	// Update is called once per frame
	void Update () {
		interval += Time.deltaTime;
		if(Input.GetKey("up") && interval >=2f){
			anim.SetBool("jump",true);
			interval = 0;
		}else{
			anim.SetBool("jump",false);
		}
		if(Input.GetKey("down") && interval >= 2f){
			anim.SetBool("slide",true);
			interval = 0;
		}else{
			anim.SetBool("slide",false);
		}
		if(Input.GetKey("right") && interval >= 2f){
			anim.SetBool("umatobi",true);
			interval = 0;
		}else{
			anim.SetBool("umatobi",false);
		}
	}
}
