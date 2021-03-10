using UnityEngine;
using System.Collections;

public class MoveScript : MonoBehaviour {
	public GameObject alpaca;
	
	private Vector3 newLocation;

	void Awake(){
		alpaca = GameObject.Find("player");
	}

	void Update(){
		if(alpaca.tag == "Player" || alpaca.tag == "Default" || alpaca.tag == "Break"){
			newLocation = new Vector3(alpaca.transform.position.x, this.transform.position.y, this.transform.position.z);
			this.transform.position = newLocation;
		}
	}
}
