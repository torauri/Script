using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo2enemy : MonoBehaviour {

	public float speed;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void FixedUpdate () {
		transform.position=new Vector3(transform.position.x,transform.position.y,transform.position.z-speed);
	}
}
