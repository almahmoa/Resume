using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {
	public bool pause = false;
	public Animator anim;
	void Start(){
		anim = GetComponent<Animator>();
	}

	void Update(){
		if(pause){
			anim.SetBool("Pause", true);
			gameObject.tag = "Pause";
		}
		if(!pause){
			gameObject.tag = "Untagged";
			anim.SetBool("Pause", false);
		}
	}

	void Pause(){
		pause = true;
		}

	void Play(){
		pause = false;
	}
}
