using UnityEngine;
using System.Collections;

public class LiveMicrophone : MonoBehaviour {

	// Microphone parameters
	private int samplingFreq = 48000;
	private int lengthSec = 1;
	private float micDelay = 0.075f;

	// Audio source on Player
	private AudioSource player;

	void Start () {
		player = GetComponent<AudioSource>();
		player.clip = Microphone.Start(null, true, lengthSec, samplingFreq);
		player.PlayDelayed(micDelay);
	}
}
