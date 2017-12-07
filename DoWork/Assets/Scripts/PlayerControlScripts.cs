using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControlScripts : MonoBehaviour {

	Slider PowerBar;
	int Maxpower = 200;
	float power = 0f;
	float PowerChange = 6.0f;
	public GameObject Weapon;
	public Transform throwPoint;


	void Start () {
		PowerBar = gameObject.GetComponentInChildren<Slider> ();
		PowerBarDisActive ();
	}
		
	void Update () {
		
		if (Input.GetKey (KeyCode.Space)) {
			PowerBar.gameObject.SetActive (true);
			power += PowerChange;
			if (power < 0 || power > 200) {
				PowerChange = -PowerChange;
			}
			PowerBar.value = power / Maxpower;

		} else if (Input.GetKeyUp (KeyCode.Space)) {
			Throw (power);
			print (power);
			power = 0;
			Invoke ("PowerBarDisActive", 2);
		} 
	}

	void Throw(float powerValue){
		Weapon = GameManager.instance.randWeapon;
		GameObject obj = Instantiate (Weapon, throwPoint.position, Weapon.transform.rotation);
		Rigidbody2D rdbd = obj.GetComponent<Rigidbody2D> ();

		rdbd.velocity = new Vector2 (0.1f*powerValue, 0.1f*powerValue);
	}

	void PowerBarDisActive(){
		PowerBar.gameObject.SetActive (false);
	}
}
