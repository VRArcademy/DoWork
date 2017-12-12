using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public List<GameObject> weaponList;
	public GameObject randWeapon;
	int randNum;

	public enum GameState{WaitToStart, GameStarted, GameEnd}
	public GameState state;

	public List<uint> playersIDList;

	void Start () {
		instance = this;
		state = GameState.WaitToStart;
	}
	

	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			RandomSpawnWeapon ();
		}
		
	}

	void RandomSpawnWeapon(){
		randNum = Random.Range (0, 3);
		print ("Random Num: " + randNum);
		randWeapon = weaponList [randNum];
	}

	void OnGUI(){
		GUI.skin.label.fontSize = 15;

		string tmp = null;
		for (int i = 0; i < playersIDList.Count; i++) {
			tmp += playersIDList [i] + ",";
		}

		GUI.Label (new Rect (10, 50, 250, 20), "PlayersID: " + tmp);
	}
}
