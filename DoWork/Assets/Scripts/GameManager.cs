using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	public static GameManager instance;
	public List<GameObject> weaponList;
	public int randNum;

	public enum GameState{WaitToStart, GameStarted, GameEnd}
	public GameState state;

	public List<uint> playersIDList;

	[SyncVar]public uint curTurnPlayerID = uint.MaxValue;
	[SyncVar]public int curTurnPlayerIndex = int.MaxValue;
	[SyncVar]bool isAllPlayerReady = false;

	void Start () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (this.gameObject);
		}
		state = GameState.WaitToStart;
	}

	void Update () {
		if (!isAllPlayerReady && playersIDList.Count == 2) {
			isAllPlayerReady = true;

			curTurnPlayerIndex = 0;
			curTurnPlayerID = playersIDList [curTurnPlayerIndex];

			state = GameState.GameStarted;
		}
	}

}