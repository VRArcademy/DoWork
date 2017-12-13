using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
	//Singleton
	public static GameManager instance;
	public GameObject Player;

	//List of Weapon
	public List<GameObject> weaponList;
	public int randNum;

	//List of Player Id
	public List<uint> playersIDList;
	[SyncVar]public uint currentPlayerId = 0;
	[SyncVar]public int curTurnPlayerIndex = int.MaxValue;

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
		playersIDList = new List<uint>();
		currentPlayerId = uint.MaxValue;
		DontDestroyOnLoad (this.gameObject);
	}

	void Start () {
		state = GameState.WaitToStart;
	}

	void Update () {
		if (!isAllPlayerReady && playersIDList.Count == 2) {
			isAllPlayerReady = true;

			curTurnPlayerIndex = 0;
			currentPlayerId = playersIDList [curTurnPlayerIndex];

			state = GameState.GameStarted;
		}
	}

}
