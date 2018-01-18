using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraV2 : MonoBehaviour {

    public GameObject followTarget;
    public float sensitivity = 1f;

    public Vector3 disGap;
    public Quaternion startRot;

    private Transform followCam;

    private Vector3 curPos, curRot, smoothRotVel;
    //private Vector3 curMousePos, preMousePos, mouseMoveDis; //use GetAxis("Mouse X")
    private Vector3 mouseMovement;
    private float xRot, yRot;

    // Use this for initialization
    void Awake()
    {
        followCam = transform.Find("Camera");
        followCam.position = followTarget.transform.position + disGap;
        curPos = followCam.position;
        mouseMovement = Vector3.zero;
        SetUp();
        //preMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    // Update is called once per frame
    void Update()
    {
        GetMouse();
        FollowTarget();
    }

    void GetMouse()
    {
        //curMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //mouseMoveDis = curMousePos - preMousePos;
        xRot += (Input.GetAxis("Mouse Y") * sensitivity);
        yRot += (Input.GetAxis("Mouse X") * sensitivity);

        //curRot = new Vector3(-xRot, yRot, 0);
        curRot = Vector3.SmoothDamp(curRot, new Vector3(-xRot, yRot, 0), ref smoothRotVel, .1f);
        transform.eulerAngles = curRot;
    }

    void FollowTarget()
    {
        curPos = followTarget.transform.position + disGap;
        followCam.position = curPos;
    }

    void SetUp()
    {
        curPos = followTarget.transform.position;
        curPos += disGap;
        //curRot = startRot;
        transform.position = curPos;
        transform.rotation = startRot;
    }

    public void TargetUpdate(GameObject obj)
    {
        followTarget = obj;
    }
}
