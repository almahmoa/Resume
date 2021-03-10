using UnityEngine;
using System.Collections;

public class SpawnScript : MonoBehaviour {
	
	public GameObject[] obj;

	void OnTriggerEnter2D(Collider2D other){
		if(other.tag == "Spawner"){
			Instantiate(obj[Random.Range(0, obj.GetLength(0))], new Vector3(this.transform.position.x, this.transform.position.y, 0), Quaternion.identity);
		}
		//find player status of mush, continue color here?
		gameObject.tag = "Destroy";
	}
}