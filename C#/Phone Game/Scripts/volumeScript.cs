using UnityEngine;
using System.Collections;

public class volumeScript : MonoBehaviour {

	public AudioListener so;

	void Start () {
		so = GetComponent<AudioListener>();
	}
	
	void Off(){
		AudioListener.volume = 0;
	}

	void On(){
		AudioListener.volume = 1;
	}
}
