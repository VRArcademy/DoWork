using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

	//For counting the number in the game scene
	[SyncVar]public int PlayerNum = int.MaxValue;

	public Canvas EndCanvas;
	public Text winTxt;

	void Start () {
		instance = this;
		state = GameState.WaitToStart;

		PlayerNum = 2;
	}

	void Update () {
		if (!isAllPlayerReady && playersIDList.Count == 2) {
			isAllPlayerReady = true;
			curTurnPlayerIndex = 0;
			curTurnPlayerID = playersIDList [curTurnPlayerIndex];

			state = GameState.GameStarted;
		}
		// check win
		if (PlayerNum <= 1) {
			state = GameState.GameEnd;
			ShowWin ();
		}
		print (playersIDList.Count);
	}

	void ShowWin(){
		EndCanvas.gameObject.SetActive (true);
		GameObject WinPlayer = GameObject.FindGameObjectWithTag ("Player");
		PlayerControlScripts PC = WinPlayer.GetComponent<PlayerControlScripts> ();
		winTxt.text = PC.pname + " Win!!!!";
	}

	public void BackToLobbyBtn(){
		Prototype.NetworkLobby.LobbyManager.s_Singleton.GoBackButton ();
	}

}
