using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WeaponScripts : NetworkBehaviour {

	[SyncVar]public uint markedID;
	public int Attack = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (this.gameObject.tag == "pickAxes") {
			Attack = 10;
		} else if (this.gameObject.tag == "Diamond") {
			Attack = 100;
		} else if (this.gameObject.tag == "DiamondAxes") {
			Attack = 15;
		}
	}
}
