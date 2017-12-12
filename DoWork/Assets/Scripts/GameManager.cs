using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

	public static GameManager instance;
	public List<GameObject> weaponList;
	//public GameObject randWeapon;
	public int randNum;

	public enum GameState{WaitToStart, GameStarted, GameEnd}
	public GameState state;

	public List<uint> playersIDList;

	void Start () {
		instance = this;
		state = GameState.WaitToStart;
	}

	void Update () {
		
	}


	/*[Command]
	void CmdRandomSpawnWeapon(){
		print ("Random Num: " + randNum);
		randWeapon = weaponList [randNum];

		RpcRandomSpawnWeapon (randNum);
	}

	[ClientRpc]
	void RpcRandomSpawnWeapon(int rand){
		randWeapon = weaponList [rand];
	}*/


}
