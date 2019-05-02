using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExplosionScript : MonoBehaviour {

	//public AudioClip explosion;
	//public AudioClip burning;
	public GameObject explosionGameObject;
	public AudioClip explosionMixed;
	AudioSource sound;

	private bool triggered;
	private bool explosionHappened;

	// Use this for initialization
	void Start () {
		triggered = false;
		explosionHappened = false;
		
		sound = GetComponent<AudioSource>();
		sound.clip = explosionMixed;
		sound.loop = false;
		sound.volume = 0.8f;
	}
	
	// Update is called once per frame
	void Update () {
		if (triggered) {
			/*if (!sound.isPlaying){
				if (!explosionHappened) {
					playSoundExplosion();
					explosionHappened = true;
				} else {
					playSoundBurning();
				}
			}*/
		}
	}

	public void triggerExplosion(){
		Debug.Log ("Explosion triggered.");
		triggered = true;
		playSoundExplosion ();
		explosionGameObject.SetActive (true);
	}
	
	public void finishExplosion(){
		if (sound.isPlaying) {
			//Debug.Log ("Stopping explosion sound playback.");
			sound.Stop ();
		}
	}
	
	public bool isTriggered(){
		return triggered;
	}

	private void playSoundExplosion(){
		//Debug.Log("Play Sound: Explosion.");
		sound.Play ();
	}

	public void resetExplosion(){
		triggered = false;
		sound.Stop ();
		explosionGameObject.SetActive (false);
	}

	/*private void playSoundExplosion(){
		Debug.Log("Play Sound: Explosion.");
		sound.clip = explosion;
		sound.loop = false;
		sound.volume = 0.5f;
		sound.Play ();
	}

	private void playSoundBurning(){
		Debug.Log("Play Sound: Burning.");
		sound.clip = burning;
		sound.loop = false;
		sound.volume = 0.5f;
		sound.Play ();
	}*/
}
