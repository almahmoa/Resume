using UnityEngine;
using System.Collections;

public class GUIScript : MonoBehaviour {

	float ran = 1f;
	float lastNum = 1.5f;
	float currentNum = 1.5f;
	public bool mush = false;
	private bool poison = false;
	private bool delay = false;

	float timer;

	public GameObject alpaca;
	private GameObject npcBool;
	private GameObject pause;
	
	void Start(){
		alpaca = GameObject.Find("player");
		npcBool = GameObject.Find("NpcBool");
		pause = GameObject.Find("pauseButton");
	}
	void Update(){
		if(npcBool.tag == "Mush"){
			delay = true;
		}
		else
			mush = false;
		if(delay){
			timer += Time.deltaTime;
		}
		if(timer > 3f){
			delay = false;
			mush = true;
			poison = true;
			timer = 0;
		}
		if(pause.tag == "Pause"){
			Time.timeScale = 0f;
		}
		if(pause.tag == "Untagged"){
			Time.timeScale = currentNum;
		}
	}

	void FixedUpdate () {
		Time.timeScale = currentNum;
		//Time.timeScale = 2f;
		//mush condition here
	
		if(mush){
			lastNum =  Mathf.Lerp(lastNum, ran, Mathf.Sin(Time.deltaTime * Mathf.PI * 0.25f));
		}
		if(!mush && poison){
			lastNum =  Mathf.Lerp(lastNum, currentNum, Mathf.Sin(Time.deltaTime * Mathf.PI * 0.25f));
		}
		if((lastNum >= currentNum - 0.1f) && !mush){
			poison = false;
			lastNum = currentNum;
		}
	}
}
