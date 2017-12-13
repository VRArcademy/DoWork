using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerControlScripts : NetworkBehaviour {


	//Slider
	Slider PowerBar;

	int Maxpower = 200;
	float power = 0f;
	float PowerChange = 6.0f;
	public GameObject Weapon;
	public Transform throwPoint;
	public Transform aimPt;
	Vector3 playerPos;

	//PlayerMovement
	public float speed = 0.1f; 
	[SyncVar]public bool facingRight;

	//Player Status
	public float playerStamina = 15.0f;
	[SyncVar]public float curPlayerStamina = 0;


	float Radius = 2.0f;
	public uint playerID;
	public int randomNum;

	public const int maxHealth = 100;
	[SyncVar(hook = "OnHealthChange")]public int Health;
	public Slider HealthBar;

	public override void OnStartLocalPlayer(){
		base.OnStartLocalPlayer ();
		if (isServer) {
			this.transform.position = new Vector2 (-12.0f, -0.75f);
		} else {
			this.transform.position = new Vector2 (12.0f, -0.75f);
			this.transform.rotation = Quaternion.Euler (0, 180.0f, 0);
			GameManager.instance.isAllPlayerReady = true;
		}
	}

	void Awake(){
		curPlayerStamina = playerStamina;
		facingRight = true;
	}

	void Start () {
		PowerBarDisActive ();
		aimPt.gameObject.SetActive (false);
		playerID = GetComponent<NetworkIdentity> ().netId.Value;
		Health = maxHealth;
	}

	void FixedUpdate () {
		playerPos = this.transform.position;

		if (isLocalPlayer && !GameManager.instance.playersIDList.Contains (playerID)) {
			CmdAddPlayer ();
		}
		if (isLocalPlayer && playerID == GameManager.instance.currentPlayerId) {
			
			aimPt.gameObject.SetActive (true);
			MovingAim ();
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
				Invoke ("TargetDisActive", 1f);
			} 
				
		}
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
		GameManager.instance.currentPlayerId = GameManager.instance.playersIDList [GameManager.instance.curTurnPlayerIndex];
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
			Vector3 aimPos = Camera.main.ScreenToWorldPoint (temp);
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
			Vector3 aimPos = Camera.main.ScreenToWorldPoint (temp);
			float dis = Vector2.Distance (aimPos, playerPos);

			if (dis > Radius) {
				Vector3 fromOriginToObject = aimPos - playerPos;
				fromOriginToObject *= Radius / dis;
				aimPos = playerPos + fromOriginToObject;
			}
			aimPt.transform.position = aimPos;
		}
	}

	//Movement && flip over when move
	void PlayerMovement(){
		float horizonSpeed = Input.GetAxis ("Horizontal") * speed;
		this.transform.Translate(horizonSpeed, 0, 0);
		CmdFlip (horizonSpeed);
		//CmdStaminaLimit ();
		if(isLocalPlayer){
			if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D)) {
				curPlayerStamina -= Time.deltaTime;
			}
		}
		if (curPlayerStamina <= 0) {
			NextTurn ();
		}
	}

	/*[Command]
	void CmdStaminaLimit(){
		if (isLocalPlayer) {
			if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.D)) {
				curPlayerStamina -= Time.deltaTime;
			}
		}
		if (curPlayerStamina <= 0) {
				NextTurn ();
		}
	}*/

	[Command]
	private void CmdFlip(float horizontal){
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
		RpcFlip (horizontal);
	}
	[ClientRpc]
	void RpcFlip(float horizontal){
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}
	//Movement && flip over when move

	//Next Turn
	void NextTurn(){
		curPlayerStamina = playerStamina;
		GameManager.instance.curTurnPlayerIndex = (GameManager.instance.curTurnPlayerIndex + 1) % 2;
		GameManager.instance.currentPlayerId = GameManager.instance.curTurnPlayerIndex [GameManager.instance.curTurnPlayerIndex];
	}

	void OnHealthChange(int health){
		HealthBar.value = (float)health / maxHealth;
	}
}
