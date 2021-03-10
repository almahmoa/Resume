using UnityEngine;
using System.Collections;

public class MushScript : MonoBehaviour {

	public Animator anim;

	public GameObject npcBool;
	public GameObject mushBool;
	bool patch = false;
	private GameObject audioS;
	//public AudioClip clip;

	void Start(){
		audioS = GameObject.Find("AudioListener");
		npcBool = GameObject.Find("NpcBool");
		mushBool = GameObject.Find("MushBool");
		npcBool.SendMessage("Stick");
		anim = GetComponent<Animator>();
		if(mushBool.tag == "Mush"){
			patch = true;
		}
		anim.SetBool("Patch", patch);
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player" || other.tag == "Break"){
			anim.SetBool("Hit", true);
			audioS.SendMessage("Mush");
			//AudioSource.PlayClipAtPoint(clip, this.transform.position);
			npcBool.SendMessage("Mush");
			mushBool.SendMessage("Mush");
			GetComponent<Rigidbody2D>().gravityScale = 3.5f;
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			GetComponent<Rigidbody2D>().AddForce(new Vector2(50, 450));
		}
		if(other.tag == "BackReset")
			npcBool.SendMessage("Untagged");
	}

	void Check(){
		anim.SetBool("Check", true);
	}
}
