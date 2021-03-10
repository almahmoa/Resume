using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchInputScript : MonoBehaviour {


	public LayerMask touchInputMask;

	private List<GameObject> touchList = new List<GameObject>();
	private GameObject[] touchesOld;
	private RaycastHit hit;

	private Vector3 fp;
	private Vector3 lp;
	private float touchDelta;
	private float dragDistance;

	private int counter = 1;
	private int i;

	void Start(){
		dragDistance = Screen.height * 1/100;
		//Debug.Log(dragDistance);
	}
	void Update(){
		if (Input.GetKeyDown(KeyCode.Escape)) { 
			Application.Quit(); 
		}

		if(Input.touchCount > 0){
			
			touchesOld = new GameObject[touchList.Count];
			touchList.CopyTo(touchesOld);
			touchList.Clear();
			
			foreach(Touch touch in Input.touches){
				Ray ray = GetComponent<Camera>().ScreenPointToRay(touch.position);
				
				if (Physics.Raycast(ray, out hit, touchInputMask)){
					GameObject recipient = hit.transform.gameObject;
					touchList.Add(recipient);
					
					if(touch.phase == TouchPhase.Began){
						recipient.SendMessage("OnTouchTap", hit.point, SendMessageOptions.DontRequireReceiver);
						
					}
				}
			}
		}
		if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)){
			
			touchesOld = new GameObject[touchList.Count];
			touchList.CopyTo(touchesOld);
			touchList.Clear();
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out hit, touchInputMask)){
				GameObject recipient = hit.transform.gameObject;
				touchList.Add(recipient);
				
				if(Input.GetMouseButtonDown(0)){
					recipient.SendMessage("OnTouchTap", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}

	void FixedUpdate(){
		
		#if UNITY_EDITOR
		
		if(Input.GetMouseButton(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)){
			
			touchesOld = new GameObject[touchList.Count];
			touchList.CopyTo(touchesOld);
			touchList.Clear();
			Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
			
			if (Physics.Raycast(ray, out hit, touchInputMask)){
				GameObject recipient = hit.transform.gameObject;
				touchList.Add(recipient);
				
				if(Input.GetMouseButtonDown(0)){
					recipient.SendMessage("OnTouchDown", hit.point, SendMessageOptions.DontRequireReceiver);
				}
				if(Input.GetMouseButtonUp(0)){
					recipient.SendMessage("OnTouchUp", hit.point, SendMessageOptions.DontRequireReceiver);
				}
				if(Input.GetMouseButton(0)){
					recipient.SendMessage("OnTouchStay", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
			foreach(GameObject g in touchesOld){
				if(!touchList.Contains(g)){
					g.SendMessage("OnTouchExit", hit.point, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		#endif
		
		if(Input.touchCount > 0){

			touchesOld = new GameObject[touchList.Count];
			touchList.CopyTo(touchesOld);
			touchList.Clear();
			i = Input.touchCount - 1;
			
			foreach(Touch touch in Input.touches){
				Ray ray = GetComponent<Camera>().ScreenPointToRay(touch.position);
				
				if (Physics.Raycast(ray, out hit, touchInputMask)){
					GameObject recipient = hit.transform.gameObject;
					touchList.Add(recipient);

					if(touch.phase == TouchPhase.Began && counter == 1){
						fp = Input.GetTouch(i).position;
						counter = 0;
					}
		
					if((touch.phase == TouchPhase.Moved) && counter == 0){
						lp =  Input.GetTouch(i).position;
						touchDelta = lp.magnitude - fp.magnitude;
						//Debug.Log(Mathf.Abs(touchDelta));
						if(Mathf.Abs(touchDelta) > dragDistance){
							if(Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y)){
								recipient.SendMessage("OnTouchSwipe", hit.point, SendMessageOptions.DontRequireReceiver);
								lp = Vector3.zero;
								fp = Vector3.zero;
								counter = 1;
							}
							else{
								recipient.SendMessage("OnTouchSwipeUp", hit.point, SendMessageOptions.DontRequireReceiver);
								lp = Vector3.zero;
								fp = Vector3.zero;
								counter = 1;
							}
						}

					}
				}
			}
		}
	}
}
