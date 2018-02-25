using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class childSimulate : MonoBehaviour {

    public GameObject followTarget;

    private GameObject empty;
    private Vector3 startRot;

    // Use this for initialization
    void Awake () {

        startRot = transform.eulerAngles;

        if ((followTarget.transform.Find("simulateChildPos")))
        {
            empty = (followTarget.transform.Find("simulateChildPos").gameObject);
        }
        else
        {
            empty = new GameObject("simulateChildPos");
            empty.transform.position = transform.position;
            //empty.transform.rotation = transform.rotation;    this not work for multi object referencing
            empty.transform.parent = followTarget.transform;
        }

    }
	
	// Update is called once per frame
	void LateUpdate ()		/******		IT CAN'T UPDATE IN TIME IF IT IS UPDATE() ********/
	{
        Track();
	}

    public void Track()
	{
		transform.position = empty.transform.position;
        //transform.eulerAngles = startRot + empty.transform.eulerAngles;
		//transform.rotation = empty.transform.rotation;
	}
}
