using UnityEngine;
using System.Collections;

public class BreakGroundScript : MonoBehaviour {
	ParticleSystem broke;
	private GameObject alpaca;
	float timer = 0f;
	public bool shattering = false;
	bool canShatter = false;

	BoxCollider2D[] box;
	CircleCollider2D[] circle;

	void Start(){
		broke = GetComponent<ParticleSystem>();
		alpaca = GameObject.Find("player");
		box = GetComponents<BoxCollider2D>();
		circle = GetComponents<CircleCollider2D>();
	}

	void Update(){
		if(alpaca.GetComponent<Rigidbody2D>().velocity.y == 0 || shattering){
			canShatter = false;
		}
		else{
			canShatter = true;
		}
		if(this.gameObject.tag == "Breaking" && !canShatter){
			shattering = true;
		}
		if(shattering && timer >= 0){
			//this.GetComponent<SpriteRenderer>().color = Color.blue;
			timer += Time.deltaTime;
			if(timer > 0.325f){
				box[0].enabled = false;
				box[1].enabled = false;
				circle[0].enabled = false;
				circle[1].enabled = false;
				alpaca.SendMessage("Break");
				this.broke.Play();
				this.GetComponent<Renderer>().material.color = Color.clear;
				timer = -1;
			}
		}
	}
	void FixedUpdate(){
		if(this.gameObject.tag == "Breaking" && canShatter){
			box[0].enabled = false;
			box[1].enabled = false;
			circle[0].enabled = false;
			circle[1].enabled = false;
			alpaca.SendMessage("Break");
			this.broke.Play();
			this.GetComponent<Renderer>().material.color = Color.clear;
			this.gameObject.tag = "Broke";
		}
	}
}