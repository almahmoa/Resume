using UnityEngine;
using System.Collections;

public class StickStickingScript : MonoBehaviour {
	public GameObject alpaca;
	public GameObject stick;
	private Vector3 newLocation;
	bool stuck = true;
	void Start () {
		alpaca = GameObject.Find("player");
	}
	

	void Update () {
		if(stuck){
			newLocation = new Vector3(alpaca.transform.position.x - 1, alpaca.transform.position.y + .5f, this.transform.position.z);
			this.transform.position = newLocation;
		}
		if(stick.tag == "Finish" && alpaca.tag == "Hit"){
			stuck = false;
		}
	}
}
