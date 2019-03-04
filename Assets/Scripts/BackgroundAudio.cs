using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class BackgroundAudio : MonoBehaviour {

    private AudioClip ZipLine_SpeedUp;

	// Use this for initialization
	void Start () {
        // Zip Line noise (128kbit_AAC)
        AudioSource audio = GetComponent<AudioSource>();
        audio.Play();
        audio.Play(44100);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
