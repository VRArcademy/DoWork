using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
<<<<<<< HEAD
	//Singleton
=======

>>>>>>> Sing
	public static GameManager instance;
	public GameObject Player;

	//List of Weapon
	public List<GameObject> weaponList;
	public int randNum;

	//List of Player Id
	public List<uint> playersIdList;
	public int curPlayerIndex = 0;
	[SyncVar]public uint currentPlayerId = 0;

	//List of Boolean
	[SyncVar]public bool isAllPlayerReady = false;

	//GameStatus
	public enum GameState{WaitToStart, GameStarted, GameEnd}
	public GameState state;

<<<<<<< HEAD
	void Awake(){
		//Singleton
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (this.gameObject);
		}

		//Players Network Id Declare
		playersIdList = new List<uint>();
		currentPlayerId = uint.MaxValue;
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
		
		if (Input.GetMouseButtonDown (0)) {
			RandomSpawnWeapon ();
=======
	public List<uint> playersIDList;

	[SyncVar]public uint curTurnPlayerID = uint.MaxValue;
	[SyncVar]public int curTurnPlayerIndex = int.MaxValue;
	[SyncVar]bool isAllPlayerReady = false;

	void Start () {
		instance = this;
		state = GameState.WaitToStart;
	}

	void Update () {
		if (!isAllPlayerReady && playersIDList.Count == 2) {
			isAllPlayerReady = true;

			curTurnPlayerIndex = 0;
			curTurnPlayerID = playersIDList [curTurnPlayerIndex];

			state = GameState.GameStarted;
>>>>>>> Sing
		}
	}

}
