using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorRotation : MonoBehaviour {

	public float speed;
	public Vector3 rotationVector;
	public Transform pickupPoint;
	public Transform rehookPoint;
	public GameObject playerRig;

	private bool go;

	// Use this for initialization
	void Start () {
		go = false;
	}

	// Update is called once per frame
	void Update () {
		
		if (playerRig.transform.position.y <= pickupPoint.position.y) {
			Debug.Log ("Reached rotor start point.");
			go = true;
		}

		if (go) {
			//Debug.Log ("Rotors spinning...");
			transform.Rotate (rotationVector * speed * Time.deltaTime);
			if (playerRig.transform.position.y >= rehookPoint.position.y) {
				Debug.Log ("Reached rotor stop point.");
				go = false;
			}
		}
		//}
	}
}
