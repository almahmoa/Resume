using UnityEngine;
using System.Collections;

public class RestartScript : MonoBehaviour {

	void OnTouchTap(){
		Application.LoadLevel(0);
	}
}
