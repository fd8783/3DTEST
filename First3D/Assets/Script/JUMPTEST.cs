using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JUMPTEST : MonoBehaviour {

    private Rigidbody bodyRB;

    public float extraGravity = 10f;
    public float jumpVel = 10f;
    Vector3 vel;

    private SphereCollider col;
    private float count = 0f;

    // Use this for initialization
    void Start () {
        bodyRB = GetComponent<Rigidbody>();
        col = GetComponentInChildren<SphereCollider>();

    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        vel = bodyRB.velocity;
        Jump();
        //vel.y -= extraGravity * Time.deltaTime;
        bodyRB.velocity = vel;
        count += Physics.gravity.y * Time.deltaTime;
        //Debug.Log(bodyRB.velocity.y.ToString("F4")+"   My: "+ count);
        //Debug.Log(col.bounds.min);
        Debug.DrawRay(col.bounds.min, -Vector3.up, Color.cyan);
        Debug.DrawRay(col.bounds.min, Vector3.right, Color.cyan);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            vel.y += jumpVel;
        }
    }
}
