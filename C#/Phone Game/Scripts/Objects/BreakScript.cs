using UnityEngine;
using System.Collections;

public class BreakScript : MonoBehaviour {
	ParticleSystem broke;
	private GameObject alpaca;

	void Start(){
		alpaca = GameObject.Find("player");
		broke = GetComponent<ParticleSystem>();
	}

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Break"){
			this.gameObject.layer = LayerMask.NameToLayer("Default");
			alpaca.SendMessage("Break");
			broke.Play();
			this.GetComponent<Renderer>().material.color = Color.clear;
		}
	}
}
