using UnityEngine;
using System.Collections;

public class GoodbyeBubbleScript : MonoBehaviour {
	public GameObject alpaca;
	public Animator anim;
	private Vector3 newLocation;

	float timer;

	float alpacaX;
	float alpacaY;

	float bubbleX;
	float bubbleY;


	void Start () {
		alpaca = GameObject.Find("player");
		bubbleX = this.transform.position.x - alpaca.transform.position.x; 
		bubbleY = this.transform.position.y - alpaca.transform.position.y;
		anim = GetComponent<Animator>();
	}
	

	void Update () {
		GetComponent<Rigidbody2D>().velocity = new Vector2(0, -5);
		alpacaX = bubbleX + alpaca.transform.position.x;
		alpacaY = bubbleY + alpaca.transform.position.y;
		
		newLocation = new Vector3(alpacaX, alpacaY, this.transform.position.z);
		this.transform.position = newLocation;
	}

	void Destroy(){
		Destroy(this.gameObject);
	}
}
