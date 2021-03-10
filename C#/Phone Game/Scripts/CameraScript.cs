using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

	float cameraY = 13;
	float downNumber;
	float upNumber;
	float currentX;
	float panning;
	float panStart = 0;

	public bool cameraDown = false;
	public bool cameraUp = false;
	public bool springTrip = false;
	public bool panCam = false;
	public bool camPanner = false;
	public bool camStart = false;
	bool setPan = false;
	
	private GameObject player;
	private GameObject npcBool;

	void Awake(){
		player = GameObject.Find("player");
		npcBool = GameObject.Find("NpcBool");
	}
	void Start(){

		float TARGET_WIDTH = 1024f;
		float TARGET_HEIGHT = 600f;
		int PIXELS_TO_UNITS = 25; // 1:1 ratio of pixels to units
		
		float desiredRatio = TARGET_WIDTH / TARGET_HEIGHT;
		float currentRatio = (float)Screen.width/(float)Screen.height;
		
		if(currentRatio >= desiredRatio)
		{
			// Our resolution has plenty of width, so we just need to use the height to determine the camera size
			Camera.main.orthographicSize = TARGET_HEIGHT / 2 / PIXELS_TO_UNITS;
		}
		else
		{
			// Our camera needs to zoom out further than just fitting in the height of the image.
			// Determine how much bigger it needs to be, then apply that to our original algorithm.
			float differenceInSize = desiredRatio / currentRatio;
			Camera.main.orthographicSize = TARGET_HEIGHT / 2 / PIXELS_TO_UNITS * differenceInSize;
		}


		if(panStart == 0){
			panStart = (this.transform.position.x);
		}
	}
	
	void Update(){

		if(player.tag == "PlayerStart"){
			camStart = true;
		}
		if(camStart)
			currentX = player.transform.position.x;

		if(player.tag == "Default"){
			camPanner = true;
			player.tag = "Player";
		}
		if(camPanner){
			panCam = false;
			currentX = player.transform.position.x;
			transform.position = new Vector3(panning, cameraY, -5);
		}
		if(((player.tag == "Player") || (player.tag == "Break")) && !panCam && !camPanner){
			currentX = player.transform.position.x;
			transform.position = new Vector3((currentX + 9f), cameraY, -5);
			panning  = 0;
		
			if((player.transform.position.y > cameraY) && springTrip)
				transform.position = new Vector3((currentX + 9f), player.transform.position.y, -5);
		}

		if(npcBool.tag == "Flip"){
			transform.rotation = Quaternion.Euler(0, 0, 180);
		}
		else{
			transform.rotation = Quaternion.Euler(0, 0, 0);
		}

		
		if(setPan){
			panning = currentX + 9f;
			this.transform.position = new Vector3(panning, cameraY, -5);
			panCam = true;
			setPan = false;
		}
		if(panCam){
			transform.position = new Vector3(panning, cameraY, -5);
		}

		if(player.tag == "Hit"){
			Hit();
		}
	}

	void FixedUpdate(){

		if(cameraDown){
			cameraY = Mathf.Lerp(cameraY, downNumber, Mathf.Sin(Time.deltaTime * Mathf.PI * 0.5f));
		}
		if((cameraY <= downNumber - .01f) || cameraUp)
			cameraDown = false;
		
		if(cameraUp){
			cameraY =  Mathf.Lerp(cameraY, upNumber, Mathf.Sin(Time.deltaTime * Mathf.PI * 0.5f));
		}
		if((cameraY >= upNumber - .01f) || cameraDown)
			cameraUp = false;

		if(panCam){
			panning =  Mathf.Lerp(panning, (currentX + 75f), Mathf.Sin(Time.deltaTime * Mathf.PI * 1.5f));
			//Debug.Log(panning + "  " + (currentX + 9f));
		}

		if(camPanner){
			//panning =  Mathf.Lerp(panning, (currentX + 9f), Mathf.Sin(Time.deltaTime * Mathf.PI * 1.5f));
			panning += Time.deltaTime * 75;
			//Debug.Log(panning + "  " + (currentX + 9f));
		}

		if((panning >= (currentX + 9f) - 1f) && camPanner){
			camPanner = false;
			player.tag = "Player";
		}
		
		if((panStart <= (currentX + 9f) - .01f) && camStart){
			player.tag = "Player";
			camStart = false;
		}
	}

	void OnTriggerEnter2D (Collider2D other){
		if(other.tag == "CameraDown"){
			downNumber = other.transform.localScale.z;
			cameraUp = false;
			cameraDown = true;
		}
		if(other.tag == "CameraUp"){
			upNumber = other.transform.localScale.z;
			cameraDown = false;
			cameraUp = true;
		}
		if(other.tag == "SpringStart"){
			springTrip = true;
		}
		if(other.tag == "SpringStop"){
			springTrip = false;
		}
		if(other.tag == "CamPan"){
			setPan = true;
		}
	}

	void Hit(){
		transform.position = new Vector3((currentX + 9f), cameraY, -5);
	}
}