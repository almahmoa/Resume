using UnityEngine;
using System.Collections;

public class FlipScript : MonoBehaviour {
	public GameObject playerControl;
	public GameObject white;
	public GameObject control;
	public Animator anim;

	public GameObject npcBool;

	private GameObject audioS;

	int num;
	bool flip = false;

	void Start () {
		npcBool = GameObject.Find("NpcBool");
		anim = GetComponent<Animator>();
		audioS = GameObject.Find("AudioListener");
		playerControl = GameObject.Find("PlayerTouchController");
	}

	void Play(){
		//transform.rotation = Quaternion.Euler(0, 0, 0);
		audioS.SendMessage("Appear");
		anim.SetBool("Flip", true);
	}
	

	void Flip() {
		num = Random.Range(1, 3);
		audioS.SendMessage("Flash");
		white.SendMessage("Play");
		if(num == 1){
			playerControl.SendMessage("Flip");
			control.SendMessage("Play");
		}
		else{
			flip = true;
			npcBool.SendMessage("FlipScreen");
			//transform.rotation = Quaternion.Euler(0, 0, 180);
		}
		//turn on white once
		//and player control reverse
	}

	void Disable(){
		transform.rotation = Quaternion.Euler(0, 0, 0);
		flip = false;
		anim.SetBool("Flip", false);
	}
}
