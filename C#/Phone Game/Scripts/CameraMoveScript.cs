using UnityEngine;
using System.Collections;

public class CameraMoveScript : MonoBehaviour {

	public GameObject cam;
	
	void Update () {
		if(cam.transform.rotation.z == 1){
			this.transform.rotation = Quaternion.Euler(0, 0, 0);
		}
	}
}
