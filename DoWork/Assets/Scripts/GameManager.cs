using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class GameManager : NetworkBehaviour {
	//Singleton
	public static GameManager instance;
	public GameObject Player;

	//List of Weapon
	public List<GameObject> weaponList;
	public GameObject randWeapon;
	int randNum;

	//List of Player Id
	public List<uint> playersIdList;
	[SyncVar]public int curPlayerIndex = 0; //no need sync
	[SyncVar]public uint currentPlayerId =  uint.MaxValue;

	//List of Boolean
	[SyncVar]public bool isAllPlayerReady = false;

	//GameStatus
	public enum GameState{WaitToStart, GameStarted, GameEnd}
	public GameState state;

	void Awake(){
		//Singleton
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (this.gameObject);
		}

		//Players Network Id Declare
		//playersIdList = new List<uint>();
		//currentPlayerId = uint.MaxValue;
		DontDestroyOnLoad (this.gameObject);
	}

	void Update () {
		if (!isServer) {
		}

		if (!isAllPlayerReady && playersIdList.Count == 2) {
			isAllPlayerReady = true;
			currentPlayerId = 0;
			currentPlayerId = playersIdList [curPlayerIndex];
		} if (!isAllPlayerReady) {
			return;
		}
	}
}
