using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FollowPathScript : MonoBehaviour {

	public GameObject positionalPlayerDummies;
	public GameObject playerInsideTracking;
	public GameObject wirePullyStart;
	public GameObject wirePullySlide;
	public GameObject cabin;
	public FanSpeedController fanController;

	public float autostartArm;
	public float autostartJump;

	// sliding the zip line
	public float slide_speedUpFactor;			//7;
	public float slide_finalSpeed;				//45

	public float preDropActionOffset;			//35
		
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
	// startingPos = start
	// 		0	   = drop point (over explosion)
	// 		1	   = watering point (slow down in water)
	// 		2	   = trigger point for helicopter arival
	// 		3	   = pickup point (helecopter pulls the player up again)
	// 		4	   = rehoop paint (player is rehocked to the zip-line)
	// 		5	   = slow down point (breaking before landing point)
	// 		6	   = landing point (final destination)

	public AudioClip SpeedUp;
	public AudioClip Loop;
	public AudioClip Stop;
	AudioSource sound;
	
	private float speed;
	private int current;
	private bool go;
	private Vector3 startingPos;
	private float[] speedUpFactors;
	private float[] finalSpeeds;
	private bool explosionTriggered;
	private bool onZipLine;
	private bool autoTriggerArmed;
	private bool initiallyTriggered;

	void Start(){
		positionalPlayerDummies.SetActive(false);
		wirePullyStart.SetActive (true);
		wirePullySlide.SetActive (false);
		startingPos = new Vector3 (1817.498f, 350.526f, 1664.079f);	// place player on staring platform
		//startingPos = transform.position;
		transform.position = startingPos;
		autoTriggerArmed = false;
		initiallyTriggered = false;
		go = false;
		onZipLine = true;
		explosionTriggered = false;
		//speed = slide_initialSpead;
		speed = 0.0f;
		current = 0;
		fanController.SetFanSpeed(0);
		
		speedUpFactors = new float[] { slide_speedUpFactor, fall_speedUpFactor, sink_slowDownFactor, sink_slowDownFactor, 0.0f,		 slide_speedUpFactor, breakFactor 		 };
		finalSpeeds    = new float[] { slide_finalSpeed,    fall_finalSpeed,    sink_finalSpeed,     sink_finalSpeed,	  liftSpeed, slide_finalSpeed   , slide_finalSpeed/5 };

		sound = GetComponent<AudioSource>();
	}

	void Update () {
		
		// movement within flight path
		if (go) {

			if (transform.position != target [current].position) {
				Vector3 pos = Vector3.MoveTowards (transform.position, target [current].position, speed * Time.deltaTime);
				GetComponent<Rigidbody> ().MovePosition (pos);

				if ((current == 0) ||
				    (current == 1) ||
				    (current == 5)) {
					// case 0 (sliding), 1 (falling) or 5 (sliding)
					// accellerate until target speed is reached

					// when player is re-hooked to the zipLine
					if (current == 5) {
						onZipLine = true;
					}

					if (current == 1){
						// fan to full power while falling
						fanController.SetFanSpeed(16);
					} else {
						// fan to relatively high power when sliding
						fanController.SetFanSpeed(12);
					}
					
					if (speed < finalSpeeds [current]) {
						speed += speedUpFactors [current];
						playSoundSpeedUp ();
					} else {
						if ((current == 0 && !explosionTriggered) || (current == 5)) {
							playSoundSliding ();
						}
					}

					// Trigger camera shake over explosion
					if (!explosionTriggered && (transform.position.z <= (target[0].position.z + preDropActionOffset))) {

						//Debug.Log("Triggering explosion.");
						cabin.GetComponent<ExplosionScript> ().triggerExplosion ();

						//Debug.Log ("Triggering camera shake.");
						Metadesc.CameraShake.ShakeManager.I.AddShake ("Explosion");

						explosionTriggered = true;
						playStopPlayback ();
						onZipLine = false;

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

					// (nearly) stop fan
					fanController.SetFanSpeed(1);

					if (current == 6) {
						playSoundStopping ();
					}
					
					if ((current == 2) && (cabin.GetComponent<ExplosionScript> ().isTriggered())) {
						cabin.GetComponent<ExplosionScript> ().finishExplosion();
					}

				} else {
					// case 4 (be lifted by helicopter)
					if (speed != liftSpeed) {
						speed = liftSpeed;
					}

					// fan to moderate speed
					fanController.SetFanSpeed(5);
				}

			} else if (current != (target.Length - 1)) {
				current += 1;
			} else {
				Debug.Log("Target reached. You have successfully landed!");
				go = false;
			}

		} else {
			
			// autostart-trigger
			if (!initiallyTriggered){
				if (!autoTriggerArmed && playerInsideTracking.transform.localPosition.z < autostartArm) {
					Debug.Log("Player passed the autostart-arming trigger point. Waiting for player to jump...");
					autoTriggerArmed = true;
				}
				if (autoTriggerArmed && playerInsideTracking.transform.localPosition.z > autostartJump){
					Debug.Log("Player passed the autostart-jump trigger point.");
					Debug.Log("Starting the ZipLine ride!");
					autoTriggerArmed = false;
					initiallyTriggered = true;
					trigerStart(true);
				}
			}
			
		}


		// input segemnt
		if (!go && Input.GetKeyDown("space"))
		{
			Debug.Log("> Got input: SPACE...");
			Debug.Log("Starting the ZipLine ride!");
			//go = true;
			initiallyTriggered = true;
			trigerStart(true);
		}
		if (go && Input.GetKeyDown("s"))
		{
			Debug.Log("> Got input: S...");
			Debug.Log("!!! EMERGENCY STOP !!!");
			Debug.LogWarning("!!! EMERGENCY STOP !!!");
			go = false;
			fanController.SetFanSpeed(0);
			playStopPlayback ();
		}
		if (!go && Input.GetKeyDown("r"))
		{
			Debug.Log("> Got input: R...");
			Debug.Log("Resetting the ride - back to the starting point.");
			GetComponent<Rigidbody>().MovePosition(startingPos);
			explosionTriggered = false;
			cabin.GetComponent<ExplosionScript> ().resetExplosion ();
			trigerStart (false);
		}
		if (!go && Input.GetKeyDown("x"))
		{
			Debug.Log("> Got input: X...");
			Debug.Log("Triggering explosion manually.");
			cabin.GetComponent<ExplosionScript> ().triggerExplosion ();
		}
		/*if (!go && Input.GetKeyDown("t"))
		{
			Debug.Log("> Got input: T...");
			Debug.Log("Transporting to landing platform imediately.");
			GetComponent<Rigidbody>().MovePosition(target[target.Length-1].position);
		}*/
		
		// Direct Jumps to mode-switch points
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Debug.Log("> Got input: 1...");
			Debug.Log("Transporting to corresponding waypoint imediately.");
			current = 0;
			GetComponent<Rigidbody>().MovePosition(startingPos);
			Debug.Log("New position: [" + GetComponent<Rigidbody>().position.x + ", " + GetComponent<Rigidbody>().position.y +", " + GetComponent<Rigidbody>().position.z +"]...");
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Debug.Log("> Got input: 2...");
			Debug.Log("Transporting to corresponding waypoint imediately.");
			current = 0;
			GetComponent<Rigidbody>().MovePosition(target[current].position);
			Debug.Log("New position: [" + GetComponent<Rigidbody>().position.x + ", " + GetComponent<Rigidbody>().position.y +", " + GetComponent<Rigidbody>().position.z +"]...");
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Debug.Log("> Got input: 3...");
			Debug.Log("Transporting to corresponding waypoint imediately.");
			current = 1;
			GetComponent<Rigidbody>().MovePosition(target[current].position);
			Debug.Log("New position: [" + GetComponent<Rigidbody>().position.x + ", " + GetComponent<Rigidbody>().position.y +", " + GetComponent<Rigidbody>().position.z +"]...");
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			Debug.Log("> Got input: 4...");
			Debug.Log("Transporting to corresponding waypoint imediately.");
			current = 2;
			GetComponent<Rigidbody>().MovePosition(target[current].position);
			Debug.Log("New position: [" + GetComponent<Rigidbody>().position.x + ", " + GetComponent<Rigidbody>().position.y +", " + GetComponent<Rigidbody>().position.z +"]...");
		}
		if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			Debug.Log("> Got input: 5...");
			Debug.Log("Transporting to corresponding waypoint imediately.");
			current = 3;
			GetComponent<Rigidbody>().MovePosition(target[current].position);
			Debug.Log("New position: [" + GetComponent<Rigidbody>().position.x + ", " + GetComponent<Rigidbody>().position.y +", " + GetComponent<Rigidbody>().position.z +"]...");
		}
		if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			Debug.Log("> Got input: 6...");
			Debug.Log("Transporting to corresponding waypoint imediately.");
			current = 4;
			GetComponent<Rigidbody>().MovePosition(target[current].position);
			Debug.Log("New position: [" + GetComponent<Rigidbody>().position.x + ", " + GetComponent<Rigidbody>().position.y +", " + GetComponent<Rigidbody>().position.z +"]...");
		}
		if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			Debug.Log("> Got input: 7...");
			Debug.Log("Transporting to corresponding waypoint imediately.");
			current = 5;
			GetComponent<Rigidbody>().MovePosition(target[current].position);
			Debug.Log("New position: [" + GetComponent<Rigidbody>().position.x + ", " + GetComponent<Rigidbody>().position.y +", " + GetComponent<Rigidbody>().position.z +"]...");
		}
		if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			Debug.Log("> Got input: 8...");
			Debug.Log("Transporting to corresponding waypoint imediately.");
			current = 6;
			GetComponent<Rigidbody>().MovePosition(target[current].position);
			Debug.Log("New position: [" + GetComponent<Rigidbody>().position.x + ", " + GetComponent<Rigidbody>().position.y +", " + GetComponent<Rigidbody>().position.z +"]...");
		}

	}

	private void trigerStart(bool start){
		go = start;
		wirePullyStart.SetActive (!start);
		wirePullySlide.SetActive (start);
	}

	public void OnDestroy() {
		fanController.SetFanSpeed (0);
		fanController.Close();
	}
	
	public bool isGoing(){
		return go;
	}


	// Sound FX
	private void playSoundSpeedUp(){
		if (onZipLine) {
			sound.clip = SpeedUp;
			sound.loop = false;
			sound.mute = false;
			sound.volume = 0.5f;
			sound.Play ();
		}
	}

	private void playSoundSliding(){
		if (onZipLine && !sound.isPlaying) {
			sound.clip = Loop;
			sound.loop = true;
			sound.mute = false;
			sound.volume = 0.5f;
			sound.Play ();
		}
	}

	private void playSoundStopping(){
		if (onZipLine) {
			sound.Stop ();
			sound.clip = Stop;
			sound.loop = false;
			sound.mute = false;
			sound.volume = 0.5f;
			sound.Play ();
		}
	}

	private void playStopPlayback(){
		//Debug.Log ("Stopping playback imediately!");
		sound.mute = true;
		sound.Stop ();
		sound.clip = null;
		sound.loop = false;
		sound.volume = 0.0f;
	}

	public bool isUnderwater(){
		if (transform.position.y <= 102.75f) {
			return true;
		} else {
			return false;
		}
	}
}
