using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_create : MonoBehaviour {

	public GameObject[] enemyPres = new GameObject[3];
	public float interval;
	private float timeEpi=0;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		timeEpi += Time.deltaTime;
		if(timeEpi>=interval){
			int i=(int)Random.Range(0,3);
			Instantiate(enemyPres[i],new Vector3(transform.position.x,transform.position.y,transform.position.z),transform.rotation);
			timeEpi=0;
		}
	}
}
