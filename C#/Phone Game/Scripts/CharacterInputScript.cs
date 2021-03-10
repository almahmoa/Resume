using UnityEngine;
using System.Collections;

public class CharacterInputScript : MonoBehaviour {

	public GameObject alpaca;
	//public GameObject npcBool;
	bool command = false;
	bool reverse = false;

	void Awake(){
		alpaca.SetActive(false);
	}

	void Start(){
		//npcBool = GameObject.Find("NpcBool");
	}

	void Update(){
		if(alpaca.tag == "Player"){
			command = true;
			alpaca.SendMessage("Command");
		}
		//if(npcBool.tag == "Flip")
			//reverse = true;
		//if(npcBool.tag != "Flip")
			//reverse = false;
		if(alpaca.tag == "Untagged"){
			if(Input.GetButtonDown("Jump")|| Input.GetButtonDown("Dash")){
				alpaca.SetActive(true);
				alpaca.tag = "PlayerStart";
			}
		}
	}

	void OnTouchDown(){
	}

	void OnTouchSwipe(){
		if(alpaca.tag == "Untagged"){
			alpaca.SetActive(true);
			alpaca.tag = "PlayerStart";
		}
		if(command && !reverse)
			alpaca.SendMessage("Swipe");
		if(command && reverse)
			alpaca.SendMessage("Jump");
	}

	void OnTouchSwipeUp() {
		if(alpaca.tag == "Untagged"){
			alpaca.SetActive(true);
			alpaca.tag = "PlayerStart";
		}
		if(command && !reverse)
			alpaca.SendMessage("Jump");
		if(command && reverse)
			alpaca.SendMessage("Swipe");
	}
	
	void OnTouchStay() {
	}
	
	void OnTouchExit() {
	}
	void Flip(){
		reverse = true;
	}
	void Normal(){
		reverse = false;
	}
}
