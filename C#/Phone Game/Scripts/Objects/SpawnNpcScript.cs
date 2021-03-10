using UnityEngine;
using System.Collections;

public class SpawnNpcScript : MonoBehaviour {
	
	public GameObject[] obj;
	public GameObject[] npc;

	public GameObject npcBool;

	private int ranNum = 0;
	private bool setNpc;
	private bool path = true;

	void Awake(){
		npcBool = GameObject.Find("NpcBool");
	}


	void Start(){
		if(npcBool.tag != "Untagged"){
			ranNum = 1;
		}
		else{
			ranNum = Random.Range(0, 4);
		}
		Path();
	}
	
	void Path(){
		if(ranNum == 2){
			npcBool.SendMessage("Stick");
			setNpc = true;
		}
		else{
			setNpc = false;
		}
	}
	
	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Spawner"){

			if(setNpc && path){
				Instantiate(npc[Random.Range(0, npc.GetLength(0))], new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
				gameObject.tag = "Destroy";
				path = false;
			}
			if(!setNpc && path){
				Instantiate(obj[Random.Range(0, obj.GetLength(0))], new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
				gameObject.tag = "Destroy";
				path = false;
			}
		}
	}
}