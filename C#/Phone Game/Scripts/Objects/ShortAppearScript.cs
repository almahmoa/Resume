using UnityEngine;
using System.Collections;

public class ShortAppearScript : MonoBehaviour {
	public Animator anim;

	void Start () {
		anim = GetComponent<Animator>();
	}

	void Play() {
		anim.SetBool("Play", true);
	}

	void Stop(){
		anim.SetBool("Play", false);
		anim.SetBool("Normal", false);
	}

	void Normal(){
		anim.SetBool("Normal", true);
	}
}
