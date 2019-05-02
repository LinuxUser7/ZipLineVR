using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MoveHelicopter : MonoBehaviour {

	public GameObject playerRig;
	public Transform helicopterArivalPoint;
	public Transform pickupPoint;
	public Transform rehookPoint;
	public float rotorSpeed;
	public float horizontalStartOffset;
	public float hookIsUpOffset;
	public float hookIsDownOffset;
	public float hookLoweringSpeed;
	public float pickUpOffset;
	public float pickUpSpeed;
	public float flightSpeed;
	public bool approachFromLowerLeft = false;

	public AudioClip helicopterSoundAir;
	public AudioClip helicopterSoundUnderwater;
	AudioSource sound;

	private bool go;
	private GameObject mainRotor;
	private GameObject rearRotor;
	private float currentOffset;
	private bool lift;
	private bool emerged;
	private float currentHorizontalOffset;

	// Use this for initialization
	void Start () {
		go = false;
		lift = false;
		emerged = false;
		mainRotor = GameObject.Find("main_rotor");
		rearRotor = GameObject.Find("Rear_rotor");
		transform.position = new Vector3(5000, 0, 5000);
		currentOffset = hookIsUpOffset;
		currentHorizontalOffset = horizontalStartOffset;
		sound = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update () {
		
		if (playerRig.transform.position.y <= helicopterArivalPoint.position.y) {
			if (!go) {
				Debug.Log ("Activating helicopter.");
			}
			go = true;
		}

		if (go && playerRig.GetComponent<FollowPathScript> ().isGoing ()) {

			// play sounds
			playSound();

			if (approachFromLowerLeft) {
				transform.position = new Vector3 (playerRig.transform.position.x + currentHorizontalOffset, playerRig.transform.position.y + currentOffset, playerRig.transform.position.z);
			} else {
				transform.position = new Vector3 (playerRig.transform.position.x - currentHorizontalOffset, playerRig.transform.position.y + currentOffset, playerRig.transform.position.z);
			}

			//Debug.Log ("Current Offser: " + currentOffset);

			// move helicopter
			if (lift) {
				// while hook is not on final position (offset), increas the offset slowly (hook moves up from player's perspective)
				if (currentOffset < pickUpOffset) {
					//Debug.Log ("Lifting hook... offset: " + currentOffset);
					currentOffset += pickUpSpeed * Time.deltaTime;
				}
			} else {
				// while hook has not reached the lowest offset, slowly lower the hook (hook moves down from player's perspective)
				if ((currentOffset > hookIsDownOffset)) {
					currentOffset -= hookLoweringSpeed * Time.deltaTime;
					//if (currentHorizontalOffset > 0) {
					//	currentHorizontalOffset = Mathf.Max(currentHorizontalOffset - flightSpeed * Time.deltaTime, 0);
						//Debug.Log ("Horiz. Offseet: " + currentHorizontalOffset);
					//}
				} else {
					//if (currentHorizontalOffset > 0) {
					//	currentHorizontalOffset = 0;
					//}
					//Debug.Log ("Lowest point for hook reached - switching mode.");
					lift = true;
					currentOffset = hookIsDownOffset;
				}
			}

			if (currentHorizontalOffset > 0) {
				currentHorizontalOffset = Mathf.Max(currentHorizontalOffset - flightSpeed * Time.deltaTime, 0);
				//Debug.Log ("Horiz. Offseet: " + currentHorizontalOffset);
			}

			// rotate rotors
			mainRotor.transform.Rotate (0, 0, rotorSpeed * Time.deltaTime);
			rearRotor.transform.Rotate (rotorSpeed * Time.deltaTime, 0, 0);
		
			if (playerRig.transform.position.y >= rehookPoint.position.y) {
				Debug.Log ("Deactivating helicopter.");
				//transform.position = new Vector3(5000, 0, 5000);
				go = false;
				//playStopPlayback ();
			}
		}
	}

	private void playSound(){
		if (playerRig.GetComponent<FollowPathScript> ().isUnderwater ()) {
			playSoundUnderwater ();
		} else {
			playSoundAir ();
		}
	}
	
	private void playSoundAir(){
		if (go && !emerged) {
			sound.clip = helicopterSoundAir;
			sound.loop = true;
			sound.mute = false;
			sound.volume = 1.0f;
			sound.Play ();
			emerged = true;
		}
	}
	
	private void playSoundUnderwater(){
		if (go && !sound.isPlaying) {
			sound.clip = helicopterSoundUnderwater;
			sound.loop = true;
			sound.mute = false;
			sound.volume = 0.6f;
			sound.Play ();
		}
	}

	private void playStopPlayback(){
		sound.mute = true;
		sound.Stop ();
		sound.clip = null;
		sound.loop = false;
		sound.volume = 0.0f;
	}

}
