using UnityEngine;
using System.Collections;

public class NewTouchInputScript : MonoBehaviour {

	public int iComfort;
	public GameObject alpaca;

	private Vector2 fp;
	private Vector2 lp;
	private float touchDelta;

	void Update () {
	
		if(Input.GetTouch(0).phase == TouchPhase.Began){
			fp = Input.GetTouch(0).position;
		}
		if(Input.GetTouch(0).phase == TouchPhase.Ended){
			lp = Input.GetTouch(0).position;
			touchDelta = lp.magnitude - fp.magnitude;

			if(Mathf.Abs(touchDelta) > iComfort){
				alpaca.SendMessage("Swipe");
			}
			else{
				alpaca.SendMessage("Jump");
			}
		}
	}
}
