using UnityEngine;
using System.Collections;

public class NpcBoolScript : MonoBehaviour {

	public GameObject alpaca;
	public GameObject controls;
	public GameObject flipFairy;
	public GameObject playerScreen;
	public GameObject audioS;

	public GameObject white;

	bool flip = false;
	bool mush = false;
	bool go = false;
	//bool stick = false;
	float timer;
	float flipTimer;
	float ranNum;

	void Start(){
		//alpaca = GameObject.Find("player");
		playerScreen = GameObject.Find("PlayerTouchController");
	}

	void FixedUpdate(){
		if(alpaca.tag == "Player"){
			go = true;
		}
		if(go){
			if(mush){
				timer += Time.deltaTime;
				if(timer >= 4f && timer < 6f){
					gameObject.tag = "Mush";
					audioS.SendMessage("Poison");
				}
					
				if(timer > 25f){
					gameObject.tag = "Untagged";
					audioS.SendMessage("Fix");
					mush = false;
				}
			}

			if(gameObject.tag == "Untagged"){
				//Flip the screen
				/*gameObject.tag = "Flip";
				timer += Time.deltaTime;
				if(timer > 50f){
					gameObject.tag = "Untagged";
					flip = false;
				}*/
				//Random Generate after time
				flipTimer += Time.deltaTime;
				if(flipTimer > 4){
					ranNum = Random.Range(0, 44);
					if(ranNum == 4){
						flipFairy.SendMessage("Play");
						gameObject.tag = "Stick";
						flip = true;
					}
					else{
						flip = false;
						flipTimer = 0;
					}
				}
			}
			if(flip){
				//gameObject.tag = "Stick";
				flipTimer = 0;
				timer += Time.deltaTime;
				//if(timer > 20f){
				if(timer > 25f){
					if(gameObject.tag == "Flip")
					{
						gameObject.tag = "Untagged";
						white.SendMessage("Play");
					}
					else{
						gameObject.tag = "Untagged";
						white.SendMessage("Play");
						controls.SendMessage("Normal");
						playerScreen.SendMessage("Normal");
					}
					flip = false;
				}

			}

			if(!mush && !flip)
				timer = 0;
			}
	}

	void Mush(){
		timer = 0;
		mush = true;
	}

	void Flip(){
		flip = true;
	}

	void Stick(){
		gameObject.tag = "Stick";
	}

	void Untagged(){
		gameObject.tag = "Untagged";
	}
	void FlipScreen(){
		gameObject.tag = "Flip";
	}
}
