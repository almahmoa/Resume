using UnityEngine;
using System.Collections;

public class RandomTotemScript : MonoBehaviour {

	public Sprite[] spr;
	private Animator anim;
	private int ran;

	void Awake(){
		anim = this.gameObject.GetComponent<Animator>();
	}

	void Start(){
		this.GetComponent<SpriteRenderer>().sprite = spr[Random.Range(0, spr.GetLength(0))];
		ran = Random.Range(0, spr.GetLength(0));
		if(ran == 1){
			anim.SetBool("Second", true);
		}
	}
}
