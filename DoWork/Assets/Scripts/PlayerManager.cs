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
	[SyncVar]public float playerHealth = 100.0f;
	[SyncVar]public float curPlayerHealth = 0;
	[SyncVar]public float playerStamina = 50.0f;
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
		facingRight = true;
	}

	void Start () {
		playerId = GetComponent<NetworkIdentity> ().netId.Value;
		
	}

	void FixedUpdate () {
		if (!isServer) {
		}

		if (isLocalPlayer) {
			if (!GameManager.instance.playersIdList.Contains (playerId)) {
				CmdAddPlayer ();
			}
			float HorizontalMove = Input.GetAxis ("Horizontal");
			PlayerMovement (HorizontalMove);
			Flip (HorizontalMove);
			CmdServerFlip (HorizontalMove);
			RpcClientFlip (HorizontalMove);
		}
	}
		
	private void PlayerMovement(float horizontal){
		this.transform.Translate (horizontal * speed, 0, 0);
	}
		
	private void Flip(float horizontal){
		if (horizontal > 0 && !facingRight || horizontal < 0 && facingRight) {
			facingRight = !facingRight;
			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}
	}

	[Command]
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
	}

	[Command]
	private void CmdOnStartFlip(){
		this.transform.localScale = new Vector2 (-1.0f, 1);
	}

	[Command]
	void CmdAddPlayer(){
		if (!GameManager.instance.playersIdList.Contains (playerId)) {
			GameManager.instance.playersIdList.Add (playerId);
			Debug.Log ("Player added: " + playerId.ToString());
		}
	}
}
