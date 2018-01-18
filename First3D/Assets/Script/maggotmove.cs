using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maggotmove : MonoBehaviour {

    private float speed, rotVel;

    private Vector2 input;

    private Rigidbody body;
    private Vector3 bodyVel;
    private Quaternion targetRot;
    private float smoothRotVel;

	// Use this for initialization
	void Awake () {
        body = GetComponent<Rigidbody>();
        bodyVel = Vector3.zero;
        input = Vector2.zero;
    }
	
	// Update is called once per frame
	void Update () {
        bodyVel = Vector3.zero;
        Move();
        Rotate();
        body.velocity = bodyVel;
        Debug.DrawRay(transform.position, bodyVel * 100, Color.green);
	}

    void Move()
    {
        /*input.x = Input.GetAxis("Vertical");
        input.y = Input.GetAxis("Horizontal");
        input = input.normalized;

        if (input != Vector2.zero)
        {
            rotVel = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, rotVel, ref smoothRotVel, 0.2f);

        }*/

        speed = Input.GetAxis("Vertical");

        bodyVel.z = speed*3;
        bodyVel = transform.TransformDirection(bodyVel);
        bodyVel.y = body.velocity.y;

    }

    void Rotate()
    {
        rotVel = Input.GetAxis("Horizontal");
        Debug.DrawRay(transform.position, transform.up*100, Color.green);
        targetRot = transform.rotation * Quaternion.AngleAxis(rotVel * 5, transform.up);
        transform.rotation = targetRot;
    }
}
