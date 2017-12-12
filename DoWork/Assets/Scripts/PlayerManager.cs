using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {

	public uint playerId;

	//PlayerMovement
	public float speed = 0.1f; 
	public bool facingRight;

	//Player Status
	public float playerHealth = 100.0f;
	[SyncVar]public float curPlayerHealth = 0;
	public float playerStamina = 15.0f;
	[SyncVar]public float curPlayerStamina = 0;
	[SyncVar]public bool refuelStamina;

	public override void OnStartLocalPlayer(){
		base.OnStartLocalPlayer ();
		if (isServer) {
			this.transform.position = new Vector2 (-6.0f, -0.75f);
		} else {
			this.transform.position = new Vector2 (6.0f, -0.75f);
		}
	}

	void Awake(){
		facingRight = true;
		refuelStamina = false;
	}

	void Start () {
		playerId = GetComponent<NetworkIdentity> ().netId.Value;
		
	}

	void FixedUpdate () {
		if (isLocalPlayer) {
			if (!GameManager.instance.playersIdList.Contains (playerId)) {
				CmdAddPlayer ();
			}
			StaminaRefuel ();
			if (GameManager.instance.currentPlayerId == playerId && curPlayerStamina >= 0) {
				PlayerMove ();
			}if (curPlayerStamina <= 0) {
				NextTurn ();
				refuelStamina = false;
			}
		}
	}
		
	//Movement && flip over when move
	void PlayerMove(){
		CmdServerPlayerMove ();
	}
		
	[Command]
	void CmdServerPlayerMove(){
		MovementControl ();
		RpcClientPlayerMove ();
	}

	[ClientRpc]
	void RpcClientPlayerMove(){
		MovementControl ();
	}
		
	void MovementControl(){
		float horizonSpeed = Input.GetAxis ("Horizontal") * speed;
		transform.Translate (horizonSpeed, 0, 0);
		if (Input.GetKey (KeyCode.A)) {
			curPlayerStamina -= Time.deltaTime;
		}if (Input.GetKey (KeyCode.D)) {
			curPlayerStamina -= Time.deltaTime;
		}
		Flip (horizonSpeed);
	}
		
	void StaminaRefuel(){
		if (refuelStamina == false) {
			curPlayerStamina = playerStamina;
			refuelStamina = true;
		}
	}

	private void PlayerMovement(float horizontal){
		this.transform.Translate (horizontal * speed, 0, 0);
		Flip (horizontal);
	}
		
	private void Flip(float horizontal){
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}

	/*[Command]
	private void CmdServerFlip(float horizontal){
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}

	[ClientRpc]
	private void RpcClientFlip(float horizontal){
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}*/

	//Movement && flip over when move

	//Next Turn
	private void NextTurn(){
		GameManager.instance.curPlayerIndex = (GameManager.instance.curPlayerIndex + 1) % 2;
		GameManager.instance.currentPlayerId = GameManager.instance.playersIdList [GameManager.instance.curPlayerIndex];
	}
		
	//PlayerId Assign
	[Command]
	void CmdAddPlayer(){
		if (!GameManager.instance.playersIdList.Contains (playerId)) {
			GameManager.instance.playersIdList.Add (playerId);
			Debug.Log ("Player added: " + playerId.ToString());
		}
	}
}
