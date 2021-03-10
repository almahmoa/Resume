using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class NewHighScoreScript : MonoBehaviour {

	public Text highScore;
	public Text score;

	float timeScore = 0;
	bool startScore = false;

	public GameObject alpaca;
	public GameObject black;

	private float text = 0;
	float playerScore = 0;

	bool displayLabel = true;

	void Awake(){
		Load();
	}

	void Start () {
		black.SetActive(false);
	}



	void Update(){
		score.text = "" + playerScore;
		if(displayLabel){
			highScore.text = "" + text;
		}
		else{
			highScore.text = "";
		}

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
				Save();
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
	}
}
[Serializable]
class PlayerData{
	public float text;
	public bool sound;
}
