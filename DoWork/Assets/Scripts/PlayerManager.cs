using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {

	public uint playerId;

	//PlayerMovement
	public float speed = 0.1f; 
	[SyncVar]public bool facingRight;

	//Player Status
	public float playerHealth = 100.0f;
	[SyncVar]public float curPlayerHealth = 0;
	public float playerStamina = 15.0f;
	[SyncVar]public float curPlayerStamina = 0;

	public override void OnStartLocalPlayer(){
		base.OnStartLocalPlayer ();
		if (isServer) {
			this.transform.position = new Vector2 (-6.0f, -0.75f);
		} else {
			this.transform.position = new Vector2 (6.0f, -0.75f);
		}
	}

	void Awake(){
		curPlayerStamina = playerStamina;
		facingRight = true;
	}

	void Start () {
		playerId = GetComponent<NetworkIdentity> ().netId.Value;
		
	}

	void FixedUpdate () {
		if (!isLocalPlayer) {
			return;
		}
		if (!GameManager.instance.playersIdList.Contains (playerId)) {
			CmdAddPlayer ();
		}
		MovementSync ();


	}

	//PlayerId Assign
	[Command]
	void CmdAddPlayer(){
		if (!GameManager.instance.playersIdList.Contains (playerId)) {
			GameManager.instance.playersIdList.Add (playerId);
			Debug.Log ("Player added: " + playerId.ToString());
		}
	}
		
	//Movement && flip over when move
	void MovementSync(){
		if (isLocalPlayer && GameManager.instance.currentPlayerId == playerId) {
			PlayerMovement ();
		} else {
			return;
		}
	}

	void PlayerMovement(){
		float horizonSpeed = Input.GetAxis ("Horizontal") * speed;
		this.transform.Translate(horizonSpeed, 0, 0);
		Flip (horizonSpeed);
		CmdFlip (horizonSpeed);
		StaminaLimit ();
		CmdStaminaLimit ();
	}

	void StaminaLimit(){
			if (Input.GetKey (KeyCode.A)) {
				curPlayerStamina -= Time.deltaTime;
			}
			if (Input.GetKey (KeyCode.D)) {
				curPlayerStamina -= Time.deltaTime;
			}
			if (curPlayerStamina <= 0) {
				NextTurn ();
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
			if (curPlayerStamina <= 0) {
				NextTurn ();
			}
	}

	void Flip(float horizontal){
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}

	[Command]
	private void CmdFlip(float horizontal){
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
		GameManager.instance.curPlayerIndex = (GameManager.instance.curPlayerIndex + 1) % 2;
		GameManager.instance.currentPlayerId = GameManager.instance.playersIdList [GameManager.instance.curPlayerIndex];
	}
}
