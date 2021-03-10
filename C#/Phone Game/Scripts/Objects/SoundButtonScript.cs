using UnityEngine;
using System.Collections;

public class SoundButtonScript : MonoBehaviour {

	public GameObject sound;
	public float count = 0;
	
	void Start(){
		sound = GameObject.Find("soundButton");
	}

	void Update(){
		if(Input.GetButtonDown("Sound")){
			OnTouchTap();
		}
	}

	
	void Reset(){
		count = 0;
	}
	
	void OnTouchTap(){
		if(sound.tag == "Untagged" && count == 0){
			sound.SendMessage("Off");
			count = 1;
		}
		if(sound.tag == "Pause" && count == 0){
			sound.SendMessage("Play");
			count = 1;
		}
		Reset();
	}
}
