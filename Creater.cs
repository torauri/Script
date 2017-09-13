using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creater : MonoBehaviour {
	public GameObject enemyPrefab;
	public int interval;
	private int frame;
	// Use this for initialization
	void Start () {
		frame = 0;
	}

	// Update is called once per frame
	void FixedUpdate () {
		frame++;
		if(frame%interval==0){
			float r=Random.Range(0.5f,1.3f);
			GameObject enemy = Instantiate(enemyPrefab,new Vector3(transform.position.x,transform.position.y+r,transform.position.z),transform.rotation);
			enemy.GetComponent<Demo2enemy>().speed=Random.Range(0.3f,0.5f);
		}
	}
}
