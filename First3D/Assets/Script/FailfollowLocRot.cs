using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailfollowLocRot : MonoBehaviour {

    public GameObject followTarget;

    public Vector3 startDir, curDir, tempDir;
    private Vector3 dirMultiply;

    public GameObject empty;

    private Transform targetTransform;

    private Vector3 startPos, targetStartPos;

    private Vector3 curRot, targetRot;

    // Use this for initialization
    void Awake () {
        if ((followTarget.transform.Find("simulateChildPos")))
        {
            empty = (followTarget.transform.Find("simulateChildPos").gameObject);
        }
        else
        {
            empty = new GameObject("simulateChildPos");
            empty.transform.parent = followTarget.transform;
            empty.transform.position = transform.position;
        }

        //targetTransform = followTarget.transform;
        //startPos = transform.position;
        //targetStartPos = targetTransform.position;

        //startDir = (startPos - targetStartPos);
        //tempDir = targetTransform.forward;
        
    }
	
	// Update is called once per frame
	void Update () {
        Track();

    }

    void Track()
    {
        //    tempDir = targetTransform.forward;
        //    Vector3 crosspro = Vector3.Cross(tempDir, targetTransform.forward);

        //    Debug.DrawRay(targetStartPos, crosspro * 100, Color.cyan);
        //    Debug.Log(crosspro.ToString("F7"));
        //    float tempangle = Vector3.Angle(tempDir, targetTransform.forward);

        //    transform.RotateAround(targetTransform.position, crosspro, tempangle);
        //    //curRot = transform.forward;
        //    //targetRot = targetTransform.forward;

        //    //transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(curRot, targetRot, 0.2f,0.3f));

        transform.position = empty.transform.position;
        transform.rotation = empty.transform.rotation;

    }
}
