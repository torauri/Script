using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animframecontroller : MonoBehaviour {
	private float framecount;
	private Animator anim;
	// Use this for initialization
	void Start () {
		framecount = 0;
		anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {

		AnimatorClipInfo[] clipInfo = anim.GetCurrentAnimatorClipInfo(0);
		AnimationClip clip = clipInfo[0].clip;

		float time = framecount/(clip.length*clip.frameRate);

		if(time>=1f){
			GameObject.Destroy(gameObject);
		}

		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		int animationHash =  stateInfo.shortNameHash;

		anim.Play(animationHash,0,time);
		framecount += 2.0f;
	}
}
