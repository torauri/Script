using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroywall : MonoBehaviour {
	void Update(){
		this.GetComponent<Rigidbody>().WakeUp();
	}
	void OnTriggerEnter(Collider collider){
		if(collider.gameObject.tag=="Enemy"){
			Object.Destroy(collider.gameObject);
		}
	}
}
