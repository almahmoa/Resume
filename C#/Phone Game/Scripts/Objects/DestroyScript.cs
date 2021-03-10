using UnityEngine;
using System.Collections;

public class DestroyScript : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Destroy")
			Destroy(other.gameObject.transform.parent.gameObject);
	}
}