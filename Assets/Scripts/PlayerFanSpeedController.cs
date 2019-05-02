using UnityEngine;
using System.Collections;

public class PlayerFanSpeedController: MonoBehaviour
{
    public FanSpeedController fanSpeedController;
    public int modeSwitch;
    private static readonly float fanFireRate = 0.5f;

    private Rigidbody player = null;
    private float MAX_SPEED = 0f;

    //private T ctrlInstance;

    // Use this for initialization
    void Start()
    {
        player = GetComponent<Rigidbody>();
        //ctrlInstance = GetComponent<FlyingControl>();
        //MAX_SPEED = FlyingControl.MAX_SPEED;

        // Set fan speed adjuster
        InvokeRepeating("AdjustFan", fanFireRate, fanFireRate);
    }

    private void AdjustFan()
    {
        float speed;
        int fanSpeed = 0;

        speed = player.velocity.magnitude;

        fanSpeed = Mathf.RoundToInt(100 * (speed / MAX_SPEED)) + 10;
        fanSpeed = Mathf.Min(fanSpeed, 100);

        //if (ctrlInstance.state == FlyingControl.GLIDING || ctrlInstance.state == FlyingControl.LANDING)
        if (modeSwitch == 1)
        {
            fanSpeed /= 2;
        }
        //else if (ctrlInstance.state == FlyingControl.LANDED)
        else
        {
            fanSpeed = 0;
        }

        Debug.Log("[FanSpeedController] Setting fan to " + fanSpeed);
        fanSpeedController.SetFanSpeed(fanSpeed);
    }
}
