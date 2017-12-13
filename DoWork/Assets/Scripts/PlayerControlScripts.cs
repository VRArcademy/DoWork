using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerControlScripts : NetworkBehaviour {

	Slider PowerBar;
	int Maxpower = 200;
	float power = 0f;
	float PowerChange = 6.0f;
	public GameObject Weapon;
	public Transform throwPoint;
	public Transform aimPt;
	Vector3 playerPos;

	public uint playerID;

	public int randomNum;

	public const int maxHealth = 100;
	[SyncVar]public int Health;

	public override void OnStartLocalPlayer(){
		
	}

	void Start () {
		PowerBar = gameObject.GetComponentInChildren<Slider> ();
		PowerBarDisActive ();

		aimPt.gameObject.SetActive (false);

		playerID = GetComponent<NetworkIdentity> ().netId.Value;

		Health = maxHealth;
	}
		
	void Update () {

		if (isLocalPlayer && !GameManager.instance.playersIDList.Contains (playerID)) {
			CmdAddPlayer ();
		}
		if (isLocalPlayer && playerID == GameManager.instance.curTurnPlayerID) {
			
			aimPt.gameObject.SetActive (true);
			CmdMovingAim ();
				
			if (Input.GetMouseButton (0)) {
				PowerBar.gameObject.SetActive (true);
				power += PowerChange;
				if (power < 0 || power > 200) {
					PowerChange = -PowerChange;
				}
				PowerBar.value = power / Maxpower;
				CmdRandWeaponNum ();

			} else if (Input.GetMouseButtonUp (0)) {
				CmdThrow (power);
				power = 0;
				Invoke ("PowerBarDisActive", 1f);
				aimPt.gameObject.SetActive (false);

			} 
			
		}

		playerPos = this.transform.position;
	}
	[Command]
	void CmdThrow(float powerValue){
		
		GameObject obj = Instantiate (Weapon, throwPoint.position, Weapon.transform.rotation);
		Rigidbody2D rdbd = obj.GetComponent<Rigidbody2D> ();
		NetworkServer.Spawn (obj);

		WeaponScripts ws = obj.GetComponent<WeaponScripts> ();
		ws.markedID = playerID;

		Vector3 direction = aimPt.position - throwPoint.position;
		direction.Normalize ();
		rdbd.velocity = new Vector2 (0.1f*direction.x*powerValue, 0.1f*direction.y*powerValue);

		NextTurn ();
	}

	void PowerBarDisActive(){
		PowerBar.gameObject.SetActive (false);
	}

	public void NextTurn(){
		GameManager.instance.curTurnPlayerIndex = (GameManager.instance.curTurnPlayerIndex + 1) % 2;
		GameManager.instance.curTurnPlayerID = GameManager.instance.playersIDList [GameManager.instance.curTurnPlayerIndex];
	}
		
	[Command]
	void CmdAddPlayer(){
		if(!GameManager.instance.playersIDList.Contains (playerID)){
			GameManager.instance.playersIDList.Add(playerID);	
		}
	}

	[Command]
	void CmdRandWeaponNum(){
		randomNum = Random.Range (0, 3);
		Weapon = GameManager.instance.weaponList[randomNum];
	}


	void OnTriggerEnter2D(Collider2D other){
		WeaponScripts WS = other.GetComponent<WeaponScripts> ();
		if (playerID != WS.markedID) {
			Health -= WS.Attack;
			Destroy (other.gameObject);
		}
	}

	[Command]
	void CmdMovingAim(){
		if (isLocalPlayer) {
			Vector3 temp = Input.mousePosition;
			/*Vector3 aimPos = aimPt.transform.position;
		      aimPos = Camera.main.ScreenToWorldPoint (temp);*/  //////Stupid mistake!!!!!!
			Vector3 aimPos = Camera.main.ScreenToWorldPoint (temp);
			//aimPt.transform.position = aimPos;

			if (aimPos.x > playerPos.x + 2.7f) {
				aimPos = new Vector2 (playerPos.x + 2.7f, aimPos.y);
			} else if (aimPos.x < playerPos.x + 0.8f) {
				aimPos = new Vector2 (playerPos.x + 0.8f, aimPos.y);
			}
			aimPt.transform.position = aimPos;
		}
	}
}
