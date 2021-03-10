using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class HighScoreScript : MonoBehaviour {
	
	private Rect position;
	private Rect positionL;
	private float text = 0;
	float playerScore = 0;
	float timeScore = 0;
	bool startScore = false;
	bool displayLabel = true;

	public GameObject alpaca;
	public GameObject black;

	void Awake(){
		//Load();
	}
	
	void Start () {
		black.SetActive(false);
		position = new Rect(175, 30.5f, 100, 25);
		positionL = new Rect(175, 65.5f, 100, 25);
	}

	public GUIStyle style = null;
	private void OnGUI(){
		if(displayLabel){
			GUI.Label(position, "" + text, style);
		}
		GUI.Label(positionL, "" + playerScore, style);
	}
	
	void Update(){
		if(startScore){
			timeScore += Time.deltaTime;
			if(timeScore >= 1){
				playerScore += 1;
				timeScore = 0;
			}
		}
	}

	void FixedUpdate(){
		if(alpaca.tag == "Player"){
			startScore = true;
		}
		if(alpaca.tag == "Hit"){
			startScore = false;
			if(playerScore > text){
				text = playerScore;
				StartCoroutine(FinalScore(.75f));
				//Save();
			}
			else{
				black.SetActive(true);
			}
		}
	}

	void OnDisable(){
		PlayerPrefs.SetInt("Score", (int)playerScore);
	}

	IEnumerator FinalScore(float delay){
		displayLabel = true;
		yield return new WaitForSeconds(delay);
		displayLabel = false;
		yield return new WaitForSeconds(delay);
		displayLabel = true;
		yield return new WaitForSeconds(delay);
		displayLabel = false;
		yield return new WaitForSeconds(delay);
		displayLabel = true;
		yield return new WaitForSeconds(delay);
		displayLabel = false;
		yield return new WaitForSeconds(delay);
		displayLabel = true;
	}
	/*
	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/dashStarplayerInfo.dat");
		PlayerData data = new PlayerData();
		data.text = text;
		bf.Serialize(file,data);
		file.Close();
		black.SetActive(true);
	}
	public void Load(){
		if(File.Exists(Application.persistentDataPath +"/dashStarplayerInfo.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/dashStarplayerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize(file);
			file.Close();
			text = data.text;
		}
	}*/
}/*
[Serializable]
class PlayerData{
	public float text;
	public bool sound;
}*/