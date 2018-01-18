using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject followTarget;
    public float sensitivity = 1f;

    public float camXRotMin= -45, camXRotMax = 60;
    public Vector3 disGap;
    public float rotCenterDis;
    public Quaternion startRot;
    public float rotSmoothVel = 0.1f;

    private Transform followCam;

    private Vector3 curPos, curRot, smoothRotVel;
    //private Vector3 curMousePos, preMousePos, mouseMoveDis; //use GetAxis("Mouse X")
    private Vector3 mouseMovement;
    private float xRot, yRot;

    // Use this for initialization
    void Awake () {
        followCam = transform.Find("Camera");
        followCam.localPosition = disGap;
        curPos = followTarget.transform.position;
        mouseMovement = Vector3.zero;
        SetUp();
        //preMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }
	
	// Update is called once per frame
	void Update () {
        GetMouse();
        FollowTarget();
    }

    void GetMouse()
    {
        //curMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //mouseMoveDis = curMousePos - preMousePos;
        xRot += (Input.GetAxis("Mouse Y") * sensitivity);
        yRot += (Input.GetAxis("Mouse X") * sensitivity);
        if (yRot > 180)
        {
            yRot -= 360;
        }
        if (yRot < -180)
        {
            yRot += 360;
        }

        xRot = Mathf.Clamp(xRot, -camXRotMax, -camXRotMin); //we use -xRot
        curRot.y = Mathf.SmoothDampAngle(curRot.y, yRot, ref smoothRotVel.y, rotSmoothVel);
        //Debug.Log(xRot);
        curRot.x = -xRot;
        //curRot = new Vector3(-xRot, yRot, 0);
        //curRot = Vector3.SmoothDamp(curRot, new Vector3(-xRot, yRot, 0), ref smoothRotVel, rotSmoothVel);
        transform.eulerAngles = curRot;
    }

    void FollowTarget()
    {
        curPos = followTarget.transform.position;
        //Debug.Log(-transform.forward);
        Debug.DrawRay(transform.position, -transform.forward, Color.green);
        curPos -= transform.forward * rotCenterDis;
        transform.position = curPos;
    }

    void SetUp()
    {
        //curPos = followTarget.transform.position;
        //curPos += disGap;
        //curRot = startRot;
        curPos -= transform.forward * rotCenterDis;
        transform.position = curPos;
        transform.rotation = startRot;
    }

    public void TargetUpdate(GameObject obj)
    {
        followTarget = obj;
    }

    public float GetYRot()
    {
        return yRot;
    }
}
