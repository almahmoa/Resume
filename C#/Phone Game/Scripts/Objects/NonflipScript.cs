using UnityEngine;
using System.Collections;

public class NonflipScript : MonoBehaviour {

	private GameObject npcBool;


	void Start () {
		npcBool = GameObject.Find("NpcBool");
	}
	

	void Update () {
		if(npcBool.tag == "Flip"){
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}
		else{
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}
}
