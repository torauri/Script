using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_controler : MonoBehaviour {

	public float speed;

	// Update is called once per frame
	void Update () {
		transform.position=new Vector3(transform.position.x,transform.position.y,transform.position.z-speed*Time.deltaTime);
	}
	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag=="analays_character"){
			GameObject.Destroy(gameObject);
		}
	}
}
