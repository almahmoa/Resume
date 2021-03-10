using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SoundScript : MonoBehaviour {

	public bool sound;
	public Animator anim;
	public GameObject audioSoul;

	void Awake(){
		Load();
	}

	void Start(){
		anim = GetComponent<Animator>();
	}

	void Update(){
		if(sound){
			anim.SetBool("Off", false);
			audioSoul.SendMessage("Off");
			gameObject.tag = "Untagged";
		}
		if(!sound){
			gameObject.tag = "Pause";
			audioSoul.SendMessage("On");
			anim.SetBool("Off", true);
		}
	}
	
	void Off(){
		sound = false;
		Save();
	}
	
	void Play(){
		sound = true;
		Save();
	}

	public void Save(){
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/dashStarplayerSoundInfo.dat");
		PlayerData dataBool = new PlayerData();
		dataBool.sound = sound;
		bf.Serialize(file,dataBool);
		file.Close();
	}
	public void Load(){
		if(File.Exists(Application.persistentDataPath +"/dashStarplayerSoundInfo.dat")){
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/dashStarplayerSoundInfo.dat", FileMode.Open);
			PlayerData dataBool = (PlayerData)bf.Deserialize(file);
			file.Close();
			sound = dataBool.sound;
		}
	}
}