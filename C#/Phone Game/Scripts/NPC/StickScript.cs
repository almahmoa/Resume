using UnityEngine;
using System.Collections;

public class StickScript : MonoBehaviour {

	public GameObject alpaca;
	public GameObject npcBool;
	public GameObject[] textL;
	public GameObject[] textR;
	public GameObject[] hello;
	public GameObject[] goodbye;

	float num;
	//float talkNum;
	float rorl;
	float timer;
	float durTimer;
	float count = 0;
	bool left;
	bool right;
	bool stuck;
	private Vector3 newLocation;

	public Animator anim;
	private GameObject audioS;

	void Start () {
		audioS = GameObject.Find("AudioListener");
		alpaca = GameObject.Find("player");
		npcBool = GameObject.Find("NpcBool");
		anim = GetComponent<Animator>();
		stuck = true;
		num = Random.Range(1, 2);
		audioS.SendMessage("Stick");
		anim.SetBool("Happy", true);
		Instantiate(hello[Random.Range(0, hello.GetLength(0))], new Vector3((this.transform.position.x + 2 + Random.Range(0, 4)), (this.transform.position.y + Random.Range(-2, 6)), 0), Quaternion.identity);
		//spawn text hello
	}
	

	void Update () {
		if(stuck){
			newLocation = new Vector3(alpaca.transform.position.x - 1, alpaca.transform.position.y + .5f, this.transform.position.z);
			this.transform.position = newLocation;
			timer += Time.deltaTime;
			if(timer > num){
				//talkNum = Random.Range(1, 4);
				TextBubble();
				timer = 0;
			}
			if(alpaca.tag == "Hit"){
				durTimer = 0;
			}
			else{
				durTimer += Time.deltaTime;
				if(durTimer >= 15f){
					count = 0;
					audioS.SendMessage("Fly");
					stuck = false;
				}
			}
		}

		if(!stuck){
			gameObject.tag = "Finish";
			if(count == 0){
				Instantiate(goodbye[Random.Range(0, goodbye.GetLength(0))], new Vector3((alpaca.transform.position.x + -1), (alpaca.transform.position.y + 5), 0), Quaternion.identity);
				count = 1;
			}
			anim.SetBool("Fly", true);
			//fly away, and proc goodbye
		}
	}

	void TextBubble(){
		/*if(talkNum == 1){
			anim.SetBool("Happy", true);
		}
		if(talkNum == 2){
			anim.SetBool("Normal", true);
		}
		if(talkNum == 3){
			anim.SetBool("Sad", true);
		}*/
		num = Random.Range(1, 2);
		count = 0;
		rorl = Random.Range(0, 5);
		audioS.SendMessage("Bubble");
		if(rorl <= 1){
			left = true;
		}
		else{
			right = true;
		}
		SpawnText();
	}

	void SpawnText(){
		if(left && count == 0){
			Instantiate(textL[Random.Range(0, textL.GetLength(0))], new Vector3((this.transform.position.x + - 3 + Random.Range(-3, 0)), (this.transform.position.y + Random.Range(-2, 5)), 0), Quaternion.identity);
			count  = 1;
		}
		if(right && count == 0){
			Instantiate(textR[Random.Range(0, textR.GetLength(0))], new Vector3((this.transform.position.x + 3 + Random.Range(0, 8)), (this.transform.position.y + Random.Range(-2, 5)), 0), Quaternion.identity);	
			count = 1;
		}
		left = false;
		right = false;
		//anim.SetBool("Happy", false);
		//anim.SetBool("Normal", false);
		//anim.SetBool("Sad", false);
	}

	void Destroy(){
		npcBool.SendMessage("Untagged");
		Destroy(this.gameObject.transform.parent.gameObject);
	}
}
