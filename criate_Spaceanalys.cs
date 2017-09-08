using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class criate_Spaceanalys : MonoBehaviour {

	public GameObject analays_sphere;
	private float i;
	private float j;
	private float k;
	// Use this for initialization
	void Start () {
		for(i=-1;i<=1;i=i+0.2f){//x座標
			for(j=0;j<=2.5;j=j+0.3f){//y座標
				for(k=0;k<=55;k=k+0.5f){//z座標

					Vector3 spherePos=new Vector3(i,j,k);
					GameObject obj = Instantiate(analays_sphere,spherePos,transform.rotation);
					obj.transform.SetParent(transform);
				}
			}
		}

	}

}
