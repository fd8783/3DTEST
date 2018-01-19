using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject followTarget;
    public float sensitivity = 1f;

    public float camXRotMin= -45, camXRotMax = 60;
    public Vector3 disGap, minDisGap;
    public float rotCenterDis;
    public Quaternion startRot;
    public float rotSmoothVel = 0.1f;

	public LayerMask blockLayer;
    private Transform followCam, camLocalPosCheck;
	private RaycastHit rayHit;
	private float rayDis;
	private Vector3 rayDir, camTargetLocalPos;

    private Vector3 targetCurPos,curPos, curRot, smoothRotVel, smoothCamMoveVel;
    //private Vector3 curMousePos, preMousePos, mouseMoveDis; //use GetAxis("Mouse X")
    private Vector3 mouseMovement;
    private float xRot, yRot;

    // Use this for initialization
    void Awake () {
        followCam = transform.Find("Camera");
		camLocalPosCheck = transform.Find("CamLocalPosCheck");
        followCam.localPosition = disGap;
		targetCurPos = followTarget.transform.position;
        mouseMovement = Vector3.zero;
		rayDis = (disGap.magnitude + rotCenterDis);
        SetUp();
		//preMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
		Debug.Log(followCam.position);
    }
	
	// Update is called once per frame
	void Update ()
	{
		targetCurPos = followTarget.transform.position;
		GetMouse();
        FollowTarget();
		CheckWallAndMoveCam();

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
        curPos = targetCurPos;
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

	void CheckWallAndMoveCam()
	{
		// I use anthor child object(camLocalPosCheck) here because I need it to check some position info (local from parent> world), 
		// if I directly change followCam's position/localposition, it cause calculation problem to following math (i.e. smoothdamp)
		camLocalPosCheck.localPosition = disGap;
		rayDir = (camLocalPosCheck.position - targetCurPos).normalized;
		Debug.DrawRay(targetCurPos, rayDir*100,Color.blue);
		if (Physics.Raycast(targetCurPos, rayDir, out rayHit, rayDis) )
		{
			Debug.DrawLine(targetCurPos, rayHit.point, Color.yellow); //raycast don't hit the object that it shoot start from it's inside space

			camLocalPosCheck.position = rayHit.point;
			//camTargetLocalPos = rayHit.point - transform.position; it work only when parent get 0 rotation ***
			camTargetLocalPos = camLocalPosCheck.localPosition;

			if (rayHit.distance <= (rotCenterDis + minDisGap.magnitude))
			{
				camTargetLocalPos.y = Mathf.Max(camTargetLocalPos.y, minDisGap.y);
				camTargetLocalPos.z = Mathf.Min(camTargetLocalPos.z, minDisGap.z);
			}
		}
		else
		{
			camTargetLocalPos = disGap;
		}
		followCam.localPosition = Vector3.SmoothDamp(followCam.localPosition, camTargetLocalPos, ref smoothCamMoveVel, rotSmoothVel/2);
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
