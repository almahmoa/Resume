using UnityEngine;
using System.Collections;

public class PauseButtonScipt : MonoBehaviour {

	public GameObject pause;
	public float count = 0;

	void Start(){
		pause = GameObject.Find("pauseButton");
	}

	void Update(){
		if(Input.GetButtonDown("Pause")){
		   OnTouchTap();
		}
	}

	void Reset(){
		count = 0;
	}

	void OnTouchTap(){
		if(pause.tag == "Untagged" && count == 0){
			pause.SendMessage("Pause");
			count = 1;
		}
		if(pause.tag == "Pause" && count == 0){
			pause.SendMessage("Play");
			count = 1;
		}
		Reset();
	}
}
