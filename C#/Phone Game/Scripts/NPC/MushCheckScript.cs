using UnityEngine;
using System.Collections;

public class MushCheckScript : MonoBehaviour {

	public GameObject mush;

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player" || other.tag == "Break"){
			mush.SendMessage("Check");
		}
	}
}
