using UnityEngine;
using System.Collections;

public class AudioScript : MonoBehaviour {
	public GameObject alpaca;
	//public GameObject npcBool;
	float timer = 0;
	float count = 0;
	bool ground = false;
	bool mush = false;

	private AudioSource[] audioBit; //order: jump, dash, mush, spring, break, grounded, springLand, stick, fly, bubble, flash
									//appear, dead
	
	void Start () {
		//alpaca = GameObject.Find("player");
		audioBit = GetComponents<AudioSource>();
	}

	void Update(){
		transform.position = new Vector3(alpaca.transform.position.x, alpaca.transform.position.y, -5);

		if(ground){
			timer += Time.deltaTime;
			if(timer > .5f){
				timer = 0;
			}
		}
		if(mush){
			if(count > 0){
				for(int i = 0; i < audioBit.GetLength(0); i++){
					audioBit[i].pitch += .075f;
				}
				count -= 1;
			}
			if(count == 0){
				mush = false;
			}
		}
	}

	void Jump () {
		audioBit[0].Play();
	}
	void Dash(){
		audioBit[1].Play();
	}
	void Mush(){
		audioBit[2].Play();
	}
	void Spring(){
		audioBit[3].Play();
	}
	void Break(){
		audioBit[4].Play();
	}
	void Grounded(){
		ground = true;
		if(timer == 0){
			audioBit[5].Play();
		}
	}
	void Ungrounded(){
		ground = false;
		timer = 1;
	}
	void SpringLand(){
		audioBit[6].Play();
	}
	void Stick(){
		audioBit[7].Play();
	}
	void Fly(){
		audioBit[8].Play();
	}
	void Bubble(){
		audioBit[9].Play();
	}
	void Flash(){
		audioBit[10].Play();
	}
	void Appear(){
		audioBit[11].Play();
	}
	void Dead(){
		audioBit[12].Play();
	}
	void Poison(){
		if(count < 4){
			for(int i = 0; i < audioBit.GetLength(0); i++){
				audioBit[i].pitch -= .075f;
			}
			count += 1;
		}
	}
	void Fix(){
		mush = true;
	}
}
