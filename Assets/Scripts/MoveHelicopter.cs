using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHelicopter : MonoBehaviour {

	public GameObject playerRig;
	public Transform pickupPoint;
	public Transform rehookPoint;
	public float rotorSpeed;
	public float finalOffset;
	public float initialOffset;
	public float pickUpSpeed;

	private bool go;
	private GameObject mainRotor;
	private GameObject rearRotor;
	private float currentOffset;

	// Use this for initialization
	void Start () {
		go = false;
		mainRotor = GameObject.Find("main_rotor");
		rearRotor = GameObject.Find("Rear_rotor");
		transform.position = new Vector3(5000, 0, 5000);
		currentOffset = initialOffset;
	}

	// Update is called once per frame
	void Update () {

		if (playerRig.transform.position.y <= pickupPoint.position.y) {
			Debug.Log ("Activating helicopter...");
			go = true;
		}

		if (go) {
			// play sounds
			//////

			// move helicopter
			transform.position = new Vector3(playerRig.transform.position.x, playerRig.transform.position.y + currentOffset, playerRig.transform.position.z);
			if (currentOffset < finalOffset) {
				currentOffset += pickUpSpeed * Time.deltaTime;
			}

			// rotate rotors
			mainRotor.transform.Rotate (0, 0, rotorSpeed * Time.deltaTime);
			rearRotor.transform.Rotate (rotorSpeed * Time.deltaTime, 0, 0);
		
			if (playerRig.transform.position.y >= rehookPoint.position.y) {
				Debug.Log ("Deactivating helicopter...");
				transform.position = new Vector3(5000, 0, 5000);
				go = false;
			}
		}
	}
}
