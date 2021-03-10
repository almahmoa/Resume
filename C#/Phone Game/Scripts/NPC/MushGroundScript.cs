using UnityEngine;
using System.Collections;

public class MushGroundScript : MonoBehaviour {
	public GameObject mushBool;
	private Animator anim;
	//float timer;
	//private bool delay = false;
	public bool mush = false;
	void Awake(){
		anim = this.gameObject.GetComponent<Animator>();
		mushBool = GameObject.Find("NpcBool");
	}

	void Update () {
		if(mushBool.tag == "Mush"){
			//delay = true;
			mush = true;
		}
		else
			mush = false;
		/*if(delay){
			timer += Time.deltaTime;
		}
		if(timer > 3f){
			delay = false;
			mush = true;
			timer = 0;
		}*/
		anim.SetBool("Mush", mush);
	}
}
