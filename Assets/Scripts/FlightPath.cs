using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FlightPath : MonoBehaviour {

    private Rigidbody player;
    private bool go;
    private bool stopping;
    private bool accelerating;
    private Vector3 startVel;
    private Vector3 startAngVel;
    private int mode;  //1 for forward, 2 for reverse, 3 for spinning
    private float stoppingFactor;
    private float spinningSpeedUpFactor;
    private float spinningSlowDownFactor;
    private Vector3 speedUpVector;
    private Vector3 slowDownVector;
    private Vector3 slidingDirection;
    private Vector3 spinningDirection;

    public int startingMode;            //1
    public float LowerSpeedUpEndZ;      //200
    public float LowerStartSlowDownZ;   //2547
    public float LowerStopingPointZ;    //2565
    public float UpperSpeedUpEndZ;      //2400
    public float UpperStartSlowDownZ;   //12.5
    public float slidingSpeed;          //45
    public float spinningSpeed;         //1.25
	public float speedUpFactor;			//7.5f;

    private float UpperStopingPointZ;

    public AudioClip SpeedUp;
    public AudioClip Loop;
    public AudioClip SlowDown;
    public AudioClip Stop;
    AudioSource audio;
     
	// Use this for initialization
	void Start () {

        player = this.GetComponent<Rigidbody>();
        UpperStopingPointZ = 2.5f;
        mode = startingMode;
        //startVel = player.velocity;
        //startVel.y = -0.06265f;
		//startVel.z = 0.55f;
        startAngVel = player.angularVelocity;
        startAngVel = Vector3.up * 0.25f * -1.0f;
        go = false;
        accelerating = true;
        stopping = false;

        //slidingDirection = new Vector3(0.0f, -0.06265f, 0.55f);
		slidingDirection = new Vector3(-100.0f, -8.32f, -103.3f);
        spinningDirection = new Vector3(0.0f, -1.0f, 0.0f);

        //speedUpFactor = 1.01f;     // Higher away from 1 is faster
        //speedUpFactor = 7.5f;
        stoppingFactor = 100.0f;
        spinningSpeedUpFactor = 0.4f;
        spinningSlowDownFactor = 1.0f;

        //speedUpVector = new Vector3(0.0f, startVel.y * speedUpFactor, startVel.z * stoppingFactor);

        audio = GetComponent<AudioSource>();

	
	}
	
	// Update is called once per frame
	void Update () {

        //if (UnityEngine.VR.ccontroller.GetPress(TriggerButton))
        //{
        //    controller.TriggerHapticPulse(1000);
        //}
        //if (deviceIndex != -1 && SteamVR_Controller.Input(deviceIndex).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            //		SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(1000);
        //if (SteamVR_Controller.Input(0).GetPressDown(SteamVR_Controller.ButtonMask.Trigger) || SteamVR_Controller.Input(1).GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        
 
		if (Input.GetKeyDown("space"))
		/*if (Input.GetKeyDown("space") ||
            SteamVR_Controller.Input(0).GetPressDown(SteamVR_Controller.ButtonMask.Trigger) ||
            SteamVR_Controller.Input(1).GetPressDown(SteamVR_Controller.ButtonMask.Trigger) )*/
        {
			Debug.Log("> Got input: SPACE...");
            go = true;
            //player.velocity = startVel;
			player.velocity = new Vector3(1.0f, 1.0f, 1.0f);
            if (mode == 3) { player.angularVelocity = startAngVel; }

            //audio.PlayOneShot(SpeedUp, 0.5f);
            audio.clip = SpeedUp; 
            audio.loop = false;
            audio.volume = 0.5f;
            //audio.Play();
        }

        if (Input.GetKeyDown("s"))
        {

            Debug.Log("Mode " + mode + ": Emergency stop!");
            go = false;
            accelerating = false;
            stopping = false;
            player.velocity = Vector3.zero;
            player.angularVelocity = Vector3.zero;
            
            audio.loop = false;
            audio.Stop();

        }

		/*
        if (go && !audio.isPlaying && Mathf.Abs(player.velocity.z) > 0.75f)
        {
            //audio.PlayOneShot(Loop, 0.5f);
            //audio.loop = true;
            audio.clip = Loop;
            audio.loop = true;
            audio.volume = 0.5f;
            //audio.Play();

        }
		*/

        if (mode == 1 && go)
        {

			if (player.transform.position.y >= LowerStopingPointZ) {
				// when not beyond stopping point

				if (player.velocity.magnitude < slidingSpeed) {
					// if final speed is not reached yet
					player.velocity += slidingDirection * speedUpFactor * Time.deltaTime;
					Debug.Log ("Mode " + mode + ": Accelerating. Speed = " + player.velocity.magnitude);
				} else {
					Debug.Log ("Mode " + mode + ": Continously Sliding. Speed = " + player.velocity.magnitude);
				}

			} else {
				Debug.Log("Mode " + mode + ": Stopping. Speed = " + player.velocity.magnitude);
			}


			/*
			  if (player.transform.position.z >= LowerStopingPointZ) { doFinalize(); }
                else
                {
                    if (player.transform.position.y >= LowerStartSlowDownZ) { doStop();}
                    else
                    {
                        //if (player.transform.position.z >= LowerSpeedUpEndZ) { doSlide(); }
                        if (player.velocity.magnitude >= slidingSpeed) { doSlide(); }
                        else
                        {
                            Debug.Log("Mode " + mode + ": Accelerating. Speed = " + player.velocity.magnitude);
                            
                        }
                    }
                }
                
                //if (go && stopping && accelerating)
                if (accelerating)
                {
                    //player.AddRelativeForce(Vector3.forward * 10.0f + Vector3.down * 0.001f, ForceMode.Impulse);
                    //player.velocity *= 1.009f;
                    player.velocity += slidingDirection * speedUpFactor * Time.deltaTime;
                }
                else
                {
                    if (stopping)
                    {
                        if (player.velocity.z > 0.0f)
                        {
                            //player.velocity *= 0.75f;
                            //player.velocity *= 0.50f;
                            player.velocity -= slidingDirection * stoppingFactor * Time.deltaTime;
                        }
                        else
                        {
                            doFinalize();
                        }
                    }
                  
                }
			*/
		}

		/*
        if (mode == 2 && go)
        {

                if (player.transform.position.z <= UpperStopingPointZ) { doFinalize(); }
                else
                {
                    if (player.transform.position.z <= UpperStartSlowDownZ) { doStop();
                    }
                    else
                    {
                        //if (player.transform.position.z <= UpperSpeedUpEndZ) { doSlide(); }
                        if (player.velocity.magnitude >= slidingSpeed) { doSlide(); }
                        else
                        {
                            Debug.Log("Mode " + mode + ": Accelerating. Speed = " + player.velocity.magnitude);

                        }
                    }
                }

                //if (go && stopping && accelerating)
                if (accelerating)
                {
                    //player.AddRelativeForce(Vector3.forward * 10.0f + Vector3.down * 0.001f, ForceMode.Impulse);
                    //player.velocity *= 1.009f;
                    player.velocity -= slidingDirection * speedUpFactor * Time.deltaTime;
                }
                else
                {
                    if (stopping)
                    {
                        if (player.velocity.z > 0)
                        {
                            //player.velocity *= 0.75f;
                            //player.velocity *= 0.50f;
                            player.velocity += slidingDirection * stoppingFactor * Time.deltaTime;
                        }
                        else
                        {
                            doFinalize();
                        }
                    }

                }

        }
		*/

		/*
        if (mode == 3 && go)
        {

            

                if (player.transform.position.z >= LowerStopingPointZ) { doFinalize(); }
                else
                {
                    if (player.transform.position.z >= LowerStartSlowDownZ) { doStop(); }
                    else
                    {
                        //if (player.transform.position.z >= LowerSpeedUpEndZ)
                        if (player.velocity.magnitude >= slidingSpeed)
                        {
                            doSlide();
                            // Rotate the object around its local X axis at 30 degree per second
                            //transform.Rotate(Vector3.down * 35.0f * Time.deltaTime);
                           // player.angularVelocity += Vector3.down * 35.0f * Time.deltaTime;

                            if (player.angularVelocity.magnitude < spinningSpeed)
                            {
                                player.angularVelocity += spinningDirection * spinningSpeedUpFactor * Time.deltaTime;
                            }

                        }
                        else
                        {
                            Debug.Log("Mode " + mode + ": Accelerating. Speed = " + player.velocity.magnitude + ", Spinn = " + player.angularVelocity.magnitude);
                            // Rotate the object around its local X axis at 10 degree per second
                            //transform.Rotate(Vector3.down * 10.0f * Time.deltaTime);
                        }
                    }
                }

                //if (go && stopping && accelerating)
                if (accelerating)
                {
                    //player.AddRelativeForce(Vector3.forward * 10.0f + Vector3.down * 0.001f, ForceMode.Impulse);
                    //player.velocity *= 1.009f;
                    player.velocity += slidingDirection * speedUpFactor * Time.deltaTime;
                    
                    if (player.angularVelocity.magnitude < spinningSpeed)
                    { 
                        player.angularVelocity += spinningDirection * spinningSpeedUpFactor * Time.deltaTime;
                    }
                }
                else
                {
                    if (stopping)
                    {
                        if (player.velocity.z > 0)
                        {
                            //player.velocity *= 0.75f;
                            //player.velocity *= 0.50f;
                            player.velocity -= slidingDirection * stoppingFactor * Time.deltaTime;
                            player.angularVelocity -= spinningDirection * spinningSlowDownFactor * Time.deltaTime;
                        }
                        else
                        {
                            doFinalize();
                        }
                    }

                }

            

        }
		*/
	
	}

    //private void doAccelerate()
    //{

    //}

    private void doSlide()
    {
        Debug.Log("Mode " + mode + ": Sliding... Speed = " + player.velocity.magnitude + ", Spinn = " + player.angularVelocity.magnitude);
        accelerating = false;
        stopping = false;
    }

    private void doStop()
    {
        Debug.Log("Mode " + mode + ": Stopping. Speed = " + player.velocity.magnitude + ", Spinn = " + player.angularVelocity.magnitude);
             
        // Only when not allready stopping..
        if (!stopping) {
            //audio.loop = false;
            //audio.Stop();
            //audio.volume = 1.0f;
            //audio.PlayOneShot(Stop, 0.5f);

            audio.clip = Stop;
            audio.loop = false;
            audio.volume = 0.5f;
            //audio.Play();


        }
        
        accelerating = false;
        stopping = true;
        //player.velocity = Vector3.zero;
        //player.angularVelocity = Vector3.zero;
    }

    private void doFinalize()
    {
        Debug.Log("Mode " + mode + ": Stopped. Speed = " + player.velocity.magnitude + ", Spinn = " + player.angularVelocity.magnitude);

        player.velocity = Vector3.zero;
        player.angularVelocity = Vector3.zero;
        go = false;
        accelerating = false;
        stopping = false;
        player.velocity = Vector3.zero;
        player.angularVelocity = Vector3.zero;
        

        StartCoroutine(WaitForNextMode());

    }

    public IEnumerator WaitForNextMode()
    {
        // see http://stackoverflow.com/questions/16929805/how-can-i-wait-for-3-seconds-and-then-set-a-bool-to-true-in-c

        go = false;

        Debug.Log("Mode Switch: startin from " + mode);

        if (mode == 1)
        {
            // going to mode 2 - Reverse!
            mode = 2;
            startVel = player.velocity;
            //startVel.y = -0.00475f;
            startVel.y = 0.06265f;
            startVel.z = -0.55f;

            //slidingDirection = new Vector3(0.0f, 0.06265f, -0.55f);

            go = false;
            stopping = false;
            accelerating = true;
        }
        else if (mode == 2)
        {
            // goint to mode 3 - Spinning right!
            mode = 3;
            startVel = player.velocity;
            //startVel.y = -0.00475f;
            startVel.y = -0.06265f;
            startVel.z = 0.55f;

            //slidingDirection = new Vector3(0.0f, -0.06265f, 0.55f);

            go = false;
            stopping = false;
            accelerating = true;
        }

        Debug.Log("Mode Switch: gone to " + mode);
        yield return new WaitForSeconds(5f); // waits 3 seconds

    }
}
