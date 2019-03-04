using UnityEngine;
using System.Collections;

public class MoveDeer : MonoBehaviour {

    //public GameObject player;
    public GameObject target;
    public float triggerPoint;
    public float speed;

    private Rigidbody deers;


	// Use this for initialization
	void Start () {

 	}
	
	// Update is called once per frame
	void Update () {



        //Debug.Log(Camera.main.gameObject.transform.position.z + " checked agains " + deers.transform.position.z);
        //if (cam.transform.position.z >= triggerPoint)
        if (Camera.main.gameObject.transform.position.z >= triggerPoint)
        {
            Debug.Log("Moving deer...");
            float step = speed * Time.deltaTime;
			this.GetComponent<Rigidbody>().transform.position = Vector3.MoveTowards(deers.transform.position, target.transform.position, step);
        }
        

	}

}
