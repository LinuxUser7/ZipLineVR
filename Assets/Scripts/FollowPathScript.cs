using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPathScript : MonoBehaviour {

	// sliding the zip line
	public float slide_speedUpFactor;			//7;
	public float slide_finalSpeed;				//45
		
	// falling down from the zip line
	public float fall_speedUpFactor;	
	public float fall_finalSpeed;
	
	// sinking under water
	public float sink_slowDownFactor;
	public float sink_finalSpeed;
	
	// being lifted back in the air from a helicopter
	public float liftSpeed;
	// final breaking bevore landing
	public float breakFactor;

	public Transform[] target;
	// 0 = start
	// 1 = DropPoint (over explosion)
	// 2 = watering point (slow down in water)
	// 3 = trigger point for helicopter arival
	// 4 = pickup point (helecopter pulls the player up again)
	// 5 = rehoop paint (player is rehocked to the zip-line)
	// 6 = slow down point (breaking before landing point)
	// 7 = landing point (final destination)

	private float speed;
	private int current;
	private bool go;
	private Vector3 startingPos;
	private float[] speedUpFactors;
	private float[] finalSpeeds;

	void Start(){
		go = false;
		//speed = slide_initialSpead;
		startingPos = transform.position;
		speed = 0.0f;
		current = 0;
		
		speedUpFactors = new float[] { slide_speedUpFactor, fall_speedUpFactor, sink_slowDownFactor, sink_slowDownFactor, 0.0f,		 slide_speedUpFactor, breakFactor 		 };
		finalSpeeds    = new float[] { slide_finalSpeed,    fall_finalSpeed,    sink_finalSpeed,     sink_finalSpeed,	  liftSpeed, slide_finalSpeed   , slide_finalSpeed/5 };
	}

	void Update () {

		// movement within flight path
		if (go) {
			/*if (speed < slide_finalSpeed && speed > slide_initialSpead) {
			Debug.Log("Changing sliding speed to " + speed);
			}*/

			//Debug.Log("State: " + current + ", speedUpFactor: " +  speedUpFactors[current] + ", finalSpeed: " + finalSpeeds[current] + " - The Current Speed Is: " + speed);
			
			if (transform.position != target [current].position) {
				Vector3 pos = Vector3.MoveTowards (transform.position, target [current].position, speed * Time.deltaTime);
				GetComponent<Rigidbody> ().MovePosition (pos);

				if ((current == 0) ||
				    (current == 1) ||
				    (current == 5)) {
					// case 0 (sliding), 1 (falling) or 5 (sliding)
					// accellerate until target speed is reached
					if (speed < finalSpeeds [current]) {
						speed += speedUpFactors [current];
					}
				} else if ((current == 2) ||
						   (current == 3) ||
						   (current == 6)) {
					// sink into water or break bevore landing
					// slow down but retain minumum speed
					if (speed > finalSpeeds [current]) {
						speed = Mathf.Max (speed - speedUpFactors [current], finalSpeeds [current]);
					} else if (speed < finalSpeeds [current]) {
						speed = finalSpeeds [current];
					}
				} else {
					// case 4 (be lifted by helicopter)
					if (speed != liftSpeed) {
						speed = liftSpeed;
					}
				}

			} else if (current != (target.Length - 1)) {
				//current = (current + 1) % target.Length;
				current += 1;
			} else {
				Debug.Log("Target reached. You have successfully landed!");
				go = false;
			}

		}

		// input segemnt
		if (!go && Input.GetKeyDown("space"))
		{
			Debug.Log("> Got input: SPACE...");
			Debug.Log("Starting the ZipLine ride!");
			go = true;
		}
		if (go && Input.GetKeyDown("s"))
		{
			Debug.Log("> Got input: S...");
			Debug.Log("!!! EMERGENCY STOP !!!");
			Debug.LogWarning("!!! EMERGENCY STOP !!!");
			go = false;
		}
		if (!go && Input.GetKeyDown("r"))
		{
			Debug.Log("> Got input: R...");
			Debug.Log("Resetting position to starting point.");
			GetComponent<Rigidbody>().MovePosition(startingPos);
		}
		if (!go && Input.GetKeyDown("t"))
		{
			Debug.Log("> Got input: T...");
			Debug.Log("Transporting to landing platform imediately.");
			GetComponent<Rigidbody>().MovePosition(target[target.Length-1].position);
		}

	}
}
