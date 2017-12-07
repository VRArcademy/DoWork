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

	void Start () {
		instance = this;
		state = GameState.WaitToStart;
	}
	

	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			RandomSpawnWeapon ();
		}
		
	}

	void RandomSpawnWeapon(){
		randNum = Random.Range (0, 3);
		print ("Random Num: " + randNum);
		randWeapon = weaponList [randNum];
	}
}
