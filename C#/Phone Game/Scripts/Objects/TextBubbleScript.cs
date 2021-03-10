using UnityEngine;
using System.Collections;

public class TextBubbleScript : MonoBehaviour {

	public GameObject alpaca;
	public GameObject stick;
	private Vector3 newLocation;

	float timer;
	bool end = true;
	bool follow = true;

	public Animator anim;

	float alpacaX;
	float alpacaY;

	float bubbleX;
	float bubbleY;

	void Start(){
		alpaca = GameObject.Find("player");
		stick = GameObject.Find("StickAttach");
		bubbleX = this.transform.position.x - alpaca.transform.position.x; 
		bubbleY = this.transform.position.y - alpaca.transform.position.y;
		anim = GetComponent<Animator>();
	}

	void Update(){

		timer += Time.deltaTime;
		if(timer >= 6){
			end = false;
			anim.SetBool("Gone", true);
		}
		if(follow){
			alpacaX = bubbleX + alpaca.transform.position.x;
			alpacaY = bubbleY + alpaca.transform.position.y;
		
			newLocation = new Vector3(alpacaX, alpacaY, this.transform.position.z);
			this.transform.position = newLocation;
		}

		if(alpaca.tag == "Break" && end){
			follow = false;
			timer = 0;
			anim.SetBool("Gone", true);
			GetComponent<Rigidbody2D>().velocity = new Vector2(5, 0);
			//rigidbody2D.AddForce(new Vector2(0,0));
		}
		if(stick.tag == "Finish"){
			timer = 0;
			end = false;
			anim.SetBool("Gone", true);
		}
		//check stick if it ended, the set anim to gone
	}

	void Destroy(){
		Destroy(this.gameObject);
	}
}