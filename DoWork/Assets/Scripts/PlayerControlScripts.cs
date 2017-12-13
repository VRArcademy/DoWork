using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerControlScripts : NetworkBehaviour {

	public Slider PowerBar;

	int Maxpower = 200;
	float power = 0f;
	float PowerChange = 6.0f;

	public GameObject Weapon;
	public Transform throwPoint;

	public Transform aimPt;
	[SyncVar]Vector3 aimPos;

	Vector3 playerPos;

	float Radius = 2.0f;

	public uint playerID;

	public int randomNum;

	public const int maxHealth = 100;
	[SyncVar(hook = "OnHealthChange")]public int Health;
	public Slider HealthBar;

	[SyncVar]public string pname = "PlayerName";
	[SyncVar]public Color playerColor = Color.white;
	public Text PnameTxt;

	void Start () {
		
		PowerBarDisActive ();

		aimPt.gameObject.SetActive (false);

		playerID = GetComponent<NetworkIdentity> ().netId.Value;

		Health = maxHealth;

		PnameTxt.text = pname;
		PnameTxt.color = playerColor;
	}
	void FixedUpdate () {

		playerPos = this.transform.position;

		if (isLocalPlayer && !GameManager.instance.playersIDList.Contains (playerID) ) {
			CmdAddPlayer ();
		}
		if (isLocalPlayer && playerID == GameManager.instance.curTurnPlayerID && GameManager.instance.state != GameManager.GameState.GameEnd) {
			
			aimPt.gameObject.SetActive (true);
			MovingAim (); //Server moving aim
			CmdMovingAim ();//client moving aim

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
				Invoke ("TargetDisActive", 1f);
			} 
				
		}

		Dead ();
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

	void TargetDisActive(){
		aimPt.gameObject.SetActive (false);
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
		
	void MovingAim(){
		if (isLocalPlayer) {
			Vector3 temp = Input.mousePosition;
			Vector3 centerPos = playerPos;
			aimPos = Camera.main.ScreenToWorldPoint (temp);
			float dis = Vector2.Distance (aimPos, playerPos);

			if (dis > Radius) {
				Vector3 fromOriginToObject = aimPos - playerPos;
				fromOriginToObject *= Radius / dis;
				aimPos = playerPos + fromOriginToObject;
			}
			aimPt.transform.position = aimPos;
		}
	}

	[Command]
	void CmdMovingAim(){
		if (isLocalPlayer) {
			Vector3 temp = Input.mousePosition;
			aimPos = Camera.main.ScreenToWorldPoint (temp);
			float dis = Vector2.Distance (aimPos, playerPos);

			if (dis > Radius) {
				Vector3 fromOriginToObject = aimPos - playerPos;
				fromOriginToObject *= Radius / dis;
				aimPos = playerPos + fromOriginToObject;
			}
			aimPt.transform.position = aimPos;
		}
	}
	void OnHealthChange(int health){
		HealthBar.value = (float)health / maxHealth;
	}

	void Dead(){
		if (Health < 0) {
			GameManager.instance.PlayerNum--;
			Destroy (gameObject);
		}
	}


}
