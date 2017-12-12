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

	//Network variables
	public uint playerID;

	public override void OnStartLocalPlayer(){

		if (isServer) {
			//1P
			//this.transform.position = new Vector3(0, -4f, 0);
			this.transform.position = new Vector2 (-4.03f, 1.63f);
			//CmdP1isready ();

		} else {
			//2P
			this.transform.position = new Vector2 (5.24f, -4.1f);
			//CmdP2isready ();
		}

	}

	void Start () {
		PowerBar = gameObject.GetComponentInChildren<Slider> ();
		PowerBarDisActive ();

		playerID = GetComponent<NetworkIdentity> ().netId.Value;
	}
		
	void Update () {
		
		if (isLocalPlayer && !GameManager.instance.playersIDList.Contains (playerID)) {
			CmdAddPlayer ();
		}

		if (isLocalPlayer) {
			if (Input.GetMouseButton (0)) {
				PowerBar.gameObject.SetActive (true);
				power += PowerChange;
				if (power < 0 || power > 200) {
					PowerChange = -PowerChange;
				}
				PowerBar.value = power / Maxpower;
			} else if (Input.GetMouseButtonUp (0)) {
				CmdThrow (power);
				power = 0;
				Invoke ("PowerBarDisActive", 1f);
			} 
		}
	}

	[Command]
	void CmdThrow(float powerValue){
		Weapon = GameManager.instance.randWeapon;
		GameObject obj = Instantiate (Weapon, throwPoint.position, Weapon.transform.rotation);
		Rigidbody2D rdbd = obj.GetComponent<Rigidbody2D> ();
		NetworkServer.Spawn (obj);

		Vector3 direction = aimPt.position - throwPoint.position;
		direction.Normalize ();
		rdbd.velocity = new Vector2 (0.1f*direction.x*powerValue, 0.1f*direction.y*powerValue);
	}

	void PowerBarDisActive(){
		PowerBar.gameObject.SetActive (false);
	}
		
	[Command]
	void CmdAddPlayer(){
		if(!GameManager.instance.playersIDList.Contains (playerID)){
			GameManager.instance.playersIDList.Add(playerID);	
		}
	}
}
