using UnityEngine;
using System.Collections;

public class FirstStickScript : MonoBehaviour {

	public GameObject stick;
	public GameObject npcBool;

	void Awake(){
		npcBool = GameObject.Find("NpcBool");
		npcBool.SendMessage("Stick");
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Player" || other.tag == "Break"){
			Instantiate(stick, new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
			Destroy(this.gameObject);
		}
	}
}
