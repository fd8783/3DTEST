using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementCtrl : MonoBehaviour {

    public GameObject followCam;
    private CameraFollow camScript;

    //movementSetting
    public float forwardSpeed = 10f, sideBackReduction = 0.5f;
    public float speedSmooth = 0.2f;
    public float rotSpeed = 0.2f, targetYRot, curYRot;
    public float groundRayLength = 0.5f; //~half body length (transform.position to body's bottom)
    public float jumpVel = 500f, jumpTime = 0.3f;
    public float extraGravity = 0f;

    private Vector3 curSmoothRotVel; //ref usage
    private Vector3 inputSpeed, curSpeed, targetSpeed, curRot, curPos;
    private Rigidbody bodyRB;
    private Quaternion targetRot;
    private bool moving = false;

    private bool isGround = false;
    private RaycastHit groundHit;

    private bool jumping = false;
    private float nextJumpTime;

    private bool aimming = false;

    private float stackCount = 0;
    private bool rewinding = false, startAtStackA = true;
    private Stack<Vector3> prePosA = new Stack<Vector3>();
    private Stack<Vector3> prePosB = new Stack<Vector3>();

    //public float debugBallSize = 1;
    //private Vector3 debugBallPos;  //debug

    // Use this for initialization
    void Awake () {
        camScript = followCam.GetComponent<CameraFollow>();
        inputSpeed = Vector3.zero;
        curSpeed = Vector3.zero;
        bodyRB = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        curPos = transform.position;
        
        GetInput();
        GetReWind();
        Aim();
        if (moving || aimming)
        {
            Rotate();
        }
        Jump();

    }

    void FixedUpdate()
    {
        //Debug.Log(bodyRB.velocity.y);
        GroundCheck();
        if (!rewinding)
        {
            Move();
            if (startAtStackA)
            {
                if (stackCount < 100)
                    prePosA.Push(transform.position);
                else
                    prePosB.Push(transform.position);
            }
            else
            {
                if (stackCount < 100)
                    prePosB.Push(transform.position);
                else
                    prePosA.Push(transform.position);
            }
            stackCount++;
            if (stackCount >= 200)
            {
                if (startAtStackA)
                {
                    stackCount -= prePosA.Count;
                    prePosA.Clear();
                }
                else
                {
                    stackCount -= prePosB.Count;
                    prePosB.Clear();
                }

                startAtStackA = !startAtStackA;
            }
        }
        else
        {
            ReWind();
        }
        //Debug.Log(stackCount + " start at A : "+startAtStackA);
    }

    void GetInput()
    {
        inputSpeed.x = Input.GetAxis("Horizontal");
        inputSpeed.z = Input.GetAxis("Vertical");
        inputSpeed = inputSpeed.normalized;
        //Debug.Log(inputSpeed.ToString("F4"));

        moving = (inputSpeed.magnitude == 0 ? false : true);
    }

    void Move()
    {
        curSpeed = bodyRB.velocity;
        //sum of this two vector' dir == the normal of it, e.g. 45 degree (middle) vector if forward*1+right*1        don't know how to explain well, just draw it
        targetSpeed = (transform.forward * inputSpeed.z * (inputSpeed.z > 0 ? 1 : sideBackReduction) + 
                         transform.right * inputSpeed.x * sideBackReduction).normalized;


        //project to ground     otherwise it jump higher if look up (I guess)   *seems don't need cause targetSpeed not affected by Camera.forward

        //curSpeed.x = Mathf.Lerp(curSpeed.x, targetSpeed.x* sideSpeed, speedSmooth);
        //curSpeed.z = Mathf.Lerp(curSpeed.z, targetSpeed.z* (targetSpeed.z > 0 ? forwardSpeed : backSpeed), speedSmooth);
        curSpeed = Vector3.Lerp(curSpeed, targetSpeed * forwardSpeed * (inputSpeed.z > 0 ? 1 : sideBackReduction), speedSmooth);
        //Debug.Log(curSpeed.ToString("F4"));

        curSpeed.y = bodyRB.velocity.y;
        if (curSpeed.y < 0)
        {
            if (isGround)
            {
                curSpeed.y = -(groundHit.distance-groundRayLength)/Time.fixedDeltaTime;
            }
            else
            {
                curSpeed.y -= extraGravity * Time.deltaTime; //default extraGravity is 0
            }
        }

        bodyRB.velocity = curSpeed;
    }

    void Rotate() //Need Improvement, smooth should be replaced by RotateToward(max rot degree/frame)
    {
        /*targetRot = Quaternion.LookRotation(followCam.transform.forward, followCam.transform.up);
        //targetRot = transform.rotation * Quaternion.AngleAxis(rotSpeed, transform.up);
        targetRot.x = transform.rotation.x;
        targetRot.z = transform.rotation.z;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotSpeed* 5);

        */
        //Debug.Log(camScript.GetYRot() + "    " + followCam.transform.localEulerAngles.y);
        targetYRot = camScript.GetYRot();
        //targetYRot = followCam.transform.localEulerAngles.y;
        //curRot = Vector3.SmoothDamp(curRot, new Vector3(curRot.x, targetYRot, curRot.z), ref curSmoothRotVel, rotSpeed * (aimming ? 0.5f : 1));
        curRot.y = Mathf.SmoothDampAngle(curRot.y, targetYRot, ref curSmoothRotVel.y, rotSpeed * (aimming? 0.5f :1 ));
        transform.eulerAngles = curRot;
        //curYRot = Mathf.SmoothDamp(curYRot, targetYRot, ref curSmoothRotVel, rotSpeed); *******NOT WORKINGG

    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGround && !jumping)
            {
                jumping = true;
                nextJumpTime = Time.time + jumpTime;
                bodyRB.AddForce(transform.up * jumpVel);
            }
            //jumping = true;
        }
        if (Input.GetButton("Jump"))
        {
            //bodyRB.AddForce(transform.up * 100);
        }
        if (Input.GetButtonUp("Jump"))
        {
            jumping = false;
        }
    }

    void Aim()
    {
        if (Input.GetButtonDown("Aim"))
        {
            aimming = true;
        }
        if (Input.GetButtonUp("Aim"))
        {
            aimming = false;
        }
    }

    void GetReWind()
    {
        if (Input.GetKeyDown("q"))
        {
            rewinding = true;
        }
        if (Input.GetKeyUp("q"))
        {
            rewinding = false;
        }
    }

    void ReWind()   //have not set that user can only rewind for 100 frames now, it may can rewind between 0- 200 frames but that's fine if I am not really going to use this function :P
                    // * it using two stack to rewind instead of list/linklist
    {
        if ( stackCount > 0)
        {
            if (startAtStackA)
            {
                transform.position = (stackCount > 100 ? prePosB.Pop() : prePosA.Pop());
            }
            else
            {
                transform.position = (stackCount > 100 ? prePosA.Pop() : prePosB.Pop());
            }
            stackCount--;
        }
    }

    void GroundCheck() //may cause stuck problem
    {
        if (Physics.Raycast(transform.position, -transform.up, out groundHit, groundRayLength + Time.fixedDeltaTime* Mathf.Abs(Mathf.Min(0,bodyRB.velocity.y)))) 
        {
            Debug.DrawLine(transform.position, groundHit.point, Color.green);
            
            isGround = true;
            if (jumping)
            {
                if (Time.time >= nextJumpTime)
                {
                    jumping = false;
                }
            }
        }
        else
        {
            
            isGround = false;
        }
        Debug.DrawLine(transform.position, transform.position + (-transform.up * (groundRayLength + Time.fixedDeltaTime * Mathf.Abs(Mathf.Min(0, bodyRB.velocity.y)))), Color.red);
    }





    /*void GroundCheck() //ShpereCast can't detect the object that's too close with the parent object, how can it be a ground check?
    {
        if (Physics.SphereCast(transform.position, debugBallSize, transform.right, out groundHit, 10f))
        {
            debugBallPos = groundHit.point;

            Debug.DrawLine(transform.position, groundHit.point, Color.green);
        }
        else
        {
            debugBallPos = transform.position + transform.right * 10;

            Debug.DrawLine(transform.position, transform.position+ transform.right * 10,Color.green);
            Debug.DrawRay(transform.position, transform.right * 10, Color.red);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(debugBallPos, debugBallSize);
    }*/
}
