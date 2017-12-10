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
	public Transform aimPt;


	void Start () {
		PowerBar = gameObject.GetComponentInChildren<Slider> ();
		PowerBarDisActive ();
	}
		
	void Update () {
		
		if (Input.GetMouseButton(0)) {
			PowerBar.gameObject.SetActive (true);
			power += PowerChange;
			if (power < 0 || power > 200) {
				PowerChange = -PowerChange;
			}
			PowerBar.value = power / Maxpower;

		} else if (Input.GetMouseButtonUp(0)) {
			Throw (power);
			//print (power);
			power = 0;
			Invoke ("PowerBarDisActive", 1f);
		} 
	}

	void Throw(float powerValue){
		Weapon = GameManager.instance.randWeapon;
		GameObject obj = Instantiate (Weapon, throwPoint.position, Weapon.transform.rotation);
		Rigidbody2D rdbd = obj.GetComponent<Rigidbody2D> ();

		/*Vector3 mPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		rdbd.velocity = new Vector2 (0.1f*powerValue, 0.1f*powerValue);
		rdbd.velocity = new Vector2 (0.05f*mPos.x*powerValue, 0.05f*mPos.y*powerValue);*/

		/*Vector3 direction = aimPt.position - throwPoint.position;
		direction.Normalize ();
		rdbd.velocity = new Vector2 (0.1f*direction.x*powerValue, 0.1f*direction.y*powerValue);*/

		Vector3 mPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 direction = mPos - throwPoint.position;
		direction.Normalize ();
		rdbd.velocity = new Vector2 (0.1f*direction.x*powerValue, 0.1f*direction.y*powerValue);
	}

	void PowerBarDisActive(){
		PowerBar.gameObject.SetActive (false);
	}
}
