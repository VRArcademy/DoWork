using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerControlScripts : NetworkBehaviour {

	public Slider PowerBar;

	public SpriteRenderer playerSprite;

	int Maxpower = 200;
	float power = 0f;
	float PowerChange = 6.0f;

	public GameObject Weapon;
	public Transform throwPoint;

	public Transform aimPt;
	Vector3 aimPos;
	Vector3 playerPos;

	//Collision Handle
	public LayerMask WhatIsGround;
	Transform GroundCheck;
	const float groundedRadius = .2f;
	public bool grounded;

	//PlayerMovement
	public float speed = 0.1f; 
	[SyncVar]public bool facingRight;

	//Player Status
	public float playerHealth = 100.0f;
	[SyncVar]public float curPlayerHealth = 0;
	public float playerStamina = 15.0f;
	[SyncVar]public float curPlayerStamina = 0;

	float Radius = 2.0f;

	public uint playerID;

	public int randomNum;

	public const int maxHealth = 100;
	[SyncVar(hook = "OnHealthChange")]public int Health;
	public Slider HealthBar;

	[SyncVar]public string pname = "PlayerName";
	[SyncVar]public Color playerColor = Color.white;
	public Text PnameTxt;

	public override void OnStartLocalPlayer(){
		base.OnStartLocalPlayer ();
	}

	void Awake(){
		playerSprite = GetComponent<SpriteRenderer> ();
		curPlayerStamina = playerStamina;
		facingRight = true;
		GroundCheck = transform.Find ("CheckGround");

	}

	void Start () {
		PowerBarDisActive ();
		aimPt.gameObject.SetActive (false);
		playerID = GetComponent<NetworkIdentity> ().netId.Value;
		Health = maxHealth;
		PnameTxt.text = pname;
		PnameTxt.color = playerColor;
	}
	void FixedUpdate () {
		if (!isLocalPlayer) {
			return;
		}

		playerPos = this.transform.position;

		if (isLocalPlayer && !GameManager.instance.playersIDList.Contains (playerID) ) {
			CmdAddPlayer ();
		}
		if (isLocalPlayer && playerID == GameManager.instance.curTurnPlayerID && GameManager.instance.state != GameManager.GameState.GameEnd) {
			aimPt.gameObject.SetActive (true);
			PlayerManagement ();
		}
		Dead ();
	}
	//Player Movement
	void PlayerManagement(){
		aimPt.gameObject.SetActive (true);
		MovementSync ();
		SlopeHandle ();
		ShootingControl ();
		MovingAim ();
	}

	void MovementSync(){
		if (curPlayerStamina >=0) {
			PlayerMovement ();
		} else {
			return;
		}
	}

	void PlayerMovement(){
		float horizonSpeed = Input.GetAxis ("Horizontal") * speed;
		this.transform.Translate(horizonSpeed, 0, 0);
		Flip (horizonSpeed);
		CmdStaminaLimit ();
	}

	void SlopeHandle(){
		grounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll (GroundCheck.position, groundedRadius, WhatIsGround);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].gameObject != gameObject) {
				grounded = true;
			}
		}

		float slopeFriction = 0.8f;
		if (grounded == true) {
			RaycastHit2D hit = Physics2D.Raycast (this.transform.position, -Vector2.up, 1f, WhatIsGround);

			if (hit.collider != null && Mathf.Abs (hit.normal.x) > 0.1f) {
				Rigidbody2D body = GetComponent<Rigidbody2D> ();
				body.velocity = new Vector2(body.velocity.x - (hit.normal.x * slopeFriction), body.velocity.y);

				Vector3 pos = this.transform.position;
				pos.y += -hit.normal.x * Mathf.Abs (body.velocity.x) * Time.deltaTime * (body.velocity.x - hit.normal.x > 0 ? 1 : -1);
				this.transform.position = pos;
			}
		}

	}

	[Command]
	void CmdStaminaLimit(){
		if (Input.GetKey (KeyCode.A)) {
			curPlayerStamina -= Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.D)) {
			curPlayerStamina -= Time.deltaTime;
		}
	}
		
	void Flip(float horizontal){
		CmdFlip (horizontal);
	}
		
	[Command]
	private void CmdFlip(float horizontal){
		RpcFlip(horizontal);
	}

	[ClientRpc]
	void RpcFlip(float horizontal){
		if(horizontal > 0){
			facingRight = true;
			playerSprite.flipX = false;
		}else if(horizontal < 0){
			facingRight = false;
			playerSprite.flipX = true;

		}
	}

	void ShootingControl(){
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

	[Command]
	void CmdThrow(float powerValue){
		GameObject obj = Instantiate (Weapon, throwPoint.position, Weapon.transform.rotation);
		Rigidbody2D rdbd = obj.GetComponent<Rigidbody2D> ();
		NetworkServer.Spawn (obj);
		WeaponScripts ws = obj.GetComponent<WeaponScripts> ();
		ws.markedID = playerID;
		Vector3 direction = this.aimPos - this.throwPoint.position;
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
	//Player Movement

	//Next Turn
	void NextTurn(){
		curPlayerStamina = playerStamina;
		GameManager.instance.curTurnPlayerIndex = (GameManager.instance.curTurnPlayerIndex + 1) % 2;
		GameManager.instance.curTurnPlayerID = GameManager.instance.playersIDList [GameManager.instance.curTurnPlayerIndex];
	}

	//AddPlayer Id
	[Command]
	void CmdAddPlayer(){
		if(!GameManager.instance.playersIDList.Contains (playerID)){
			GameManager.instance.playersIDList.Add(playerID);	
		}
	}

	void Dead(){
		if (Health < 0) {
			GameManager.instance.PlayerNum--;
			Destroy (gameObject);
		}
	}


}
