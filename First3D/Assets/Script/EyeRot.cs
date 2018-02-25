using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeRot : childSimulate {

	public Vector3 rotMin, rotMax;

	private Transform parentCam, camEnd;

	private Transform empty;
	private Vector3 startEuler, rotEuler;
	private Vector3 smoothRotVel;

	// Use this for initialization
	void Start () {
		startEuler = transform.localEulerAngles;
		parentCam = transform.root.GetComponent<movementCtrl>().followCam.transform;
		camEnd = parentCam.Find("Camera");
		empty = new GameObject("eyeTarget").transform;
		rotMin += transform.localEulerAngles;
		rotMax += transform.localEulerAngles;
	}

	// Update is called once per frame
	void LateUpdate ()      /******		IT CAN'T UPDATE IN TIME IF IT IS UPDATE() ********/
	{
		base.Track();
		Rot();
	}

	void Update()
	{
		//transform.LookAt()	
	}

	public void Rot()
	{
		empty.position = (parentCam.position - camEnd.position) *3 + parentCam.position;

		Debug.DrawRay(transform.position, transform.forward * 100, Color.red);
		transform.LookAt(empty);
		rotEuler = transform.localEulerAngles;

		//if (rotEuler.x > 0)
		//	rotEuler.x = Mathf.Min(rotEuler.x, rotMax.x);
		//else
		//	rotEuler.x = Mathf.Max(rotEuler.x, rotMin.x);

		//if (rotEuler.y > 0)
		//	rotEuler.y = Mathf.Min(rotEuler.y, rotMax.y);
		//else
		//	rotEuler.y = Mathf.Max(rotEuler.y, rotMin.y);

		if (rotEuler.x > rotMax.x && rotEuler.x <= 180)
		{
			rotEuler.x = rotMax.x;
		}
		else if (rotEuler.x < (rotMin.x+360) && rotEuler.x > 180)
		{
			rotEuler.x = rotMin.x;
		}

		if (rotEuler.y > rotMax.y && rotEuler.y <= 180)
		{
			rotEuler.y = rotMax.y;
		}
		else if (rotEuler.y < (rotMin.y+360) && rotEuler.y > 180)
		{
			rotEuler.y = rotMin.y;
		}

		transform.localEulerAngles = rotEuler;

	}
}
