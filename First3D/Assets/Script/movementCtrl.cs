using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movementCtrl : MonoBehaviour {


    public GameObject followCam;
    private CameraFollow camScript;
    private Animator anim;
    private mousePoint mouseScript;

    public bool rotateWhenAttacking = false;

    //movementSetting
    public float forwardSpeed = 10f, sideBackReduction = 0.5f, speedToAnimWalk = 0.3f;
    public float speedSmooth = 0.2f;
    public float rotSpeed = 0.2f, targetYRot, curYRot;
    public float groundRayLength = 0.5f; //~half body length (transform.position to body's bottom)
    public float jumpVel = 500f, jumpTime = 0.3f;
    public float extraGravity = 0f;

    private float animSpeed, animForwardSpeed, animRightSpeed, smoothX,smoothZ;
    private Vector3 curSmoothRotVel; //ref usage
    private Vector3 inputSpeed, curSpeed, targetSpeed, curRot, curPos;
    private Rigidbody bodyRB;
    private Quaternion targetRot;
    private bool moving = false;

    private bool isGround = false;
    private RaycastHit groundHit;

    private bool jumping = false;
    private float nextJumpTime;

    public Transform leftHand, rightHand, weapon;
    public Vector3 RHSWeaponPos, RHSWeaponRot, LHSWeaponPos, LHSWeaponRot;
    public float holdTime = 0.3f, attackTime = 0.8f;
    private float countHold, countAttack = -1f;
    private bool holding = false, waitingForAttack = false, attacking = false;
    private bool blocking = false, curRightHand = true;


    private float stackCount = 0;
    private bool rewinding = false, startAtStackA = true;
    private Stack<Vector3> prePosA = new Stack<Vector3>();
    private Stack<Vector3> prePosB = new Stack<Vector3>();

    //public float debugBallSize = 1;
    //private Vector3 debugBallPos;  //debug

    // Use this for initialization
    void Awake() {
        camScript = followCam.GetComponent<CameraFollow>();
        mouseScript = transform.Find("model").GetComponent<mousePoint>();
        inputSpeed = Vector3.zero;
        curSpeed = Vector3.zero;
        bodyRB = GetComponent<Rigidbody>();
        anim = transform.Find("model").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        curPos = transform.position;

        GetInput();
        GetReWind();
        Hold();
        Block();
        WaitingForAttack();
        if (moving || holding || blocking || attacking) //!attacking = !moving
        {
            if (rotateWhenAttacking || !attacking)  // I wrote the TTFF table in paper
            {
                Rotate();
            }
        }
        Jump();
        CheckHoldOrAttack();
        AnimState();
    }

    void FixedUpdate()
    {
		//Debug.Log(bodyRB.velocity.y);
		animSpeed = 0;
		//animForwardSpeed = 0;
		//animRightSpeed = 0;
		GroundCheck();
        if (!rewinding)
        {
            if (!attacking)
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
        }
        else
        {
            ReWind();
        }
		//Debug.Log(stackCount + " start at A : "+startAtStackA);
		anim.SetFloat("speed", animSpeed);
        anim.SetFloat("forwardSpeed", animForwardSpeed);
        anim.SetFloat("rightSpeed", animRightSpeed);
    }

    void GetInput()
    {
        inputSpeed.x = Input.GetAxis("Horizontal");
        inputSpeed.z = Input.GetAxis("Vertical");
        inputSpeed = inputSpeed.normalized;
        //Debug.Log(inputSpeed.ToString("F4"));

        moving = (inputSpeed.magnitude >= 0.1f ? true : false);
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

        curSpeed.y = 0;
        //Debug.Log(curSpeed.magnitude);
        if (curSpeed.magnitude > speedToAnimWalk)
        {
			//animForwardSpeed = Mathf.Clamp(animForwardSpeed+inputSpeed.z == 0? 0 : Mathf.Sign(inputSpeed.z), -1,1);
			//animRightSpeed = Mathf.Clamp(animRightSpeed+ inputSpeed.x == 0? 0 : Mathf.Sign(inputSpeed.x), -1, 1);
			//animForwardSpeed = inputSpeed.z == 0 ? 0 : Mathf.Sign(inputSpeed.z);
			//animRightSpeed = inputSpeed.x == 0 ? 0 : Mathf.Sign(inputSpeed.x);
		}
        animForwardSpeed = Mathf.SmoothDamp(animForwardSpeed, inputSpeed.z == 0 ? 0 : Mathf.Sign(inputSpeed.z), ref smoothZ, speedSmooth);
        animRightSpeed = Mathf.SmoothDamp(animRightSpeed, inputSpeed.x == 0 ? 0 : Mathf.Sign(inputSpeed.x), ref smoothX, speedSmooth);

        curSpeed.y = bodyRB.velocity.y;
        if (curSpeed.y < 0)
        {
            if (isGround)
            {
				//curSpeed.y = -(groundHit.distance-groundRayLength)/Time.fixedDeltaTime;
				curSpeed.y = (groundHit.distance - groundRayLength) * Time.fixedDeltaTime* 1.1f;
			}
            else
            {
                curSpeed.y -= extraGravity * Time.deltaTime; //default extraGravity is 0
            }
            
        }
        
        bodyRB.velocity = curSpeed;
		animSpeed = inputSpeed.magnitude;
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
        curRot.y = Mathf.SmoothDampAngle(curRot.y, targetYRot, ref curSmoothRotVel.y, rotSpeed * (holding || blocking? 0.5f :1 ));
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

    void AnimState()
    {
        anim.SetBool("holding", holding);
        anim.SetBool("blocking", blocking);
    }

    /*void Hold()
    {
        if (Input.GetButtonDown("Hold") && !blocking && Time.time - countAttack >= attackTime)
        {
            StartHold();
        }
        if (Input.GetButtonUp("Hold"))
        {
            if (holding) 
            {
                if (Time.time - countHold >= holdTime)
                {
                    //holded = true;
                    countAttack = Time.time;
                    holding = false;
                    anim.SetTrigger("attack");
                    anim.SetTrigger("attackleg");
                    StartCoroutine(AutoHoldUp(attackTime));
                }
                else
                {
                    countAttack = Time.time + (holdTime - (Time.time - countHold));
                    StartCoroutine(AutoHoldDown(holdTime - (Time.time - countHold)));
                }
            }
            else // !holding  = get interrupted
            {
                //holded = true;
            }
		}
    }

    void StartHold()
    {
        holding = true;
        countHold = Time.time;
    }
    
    IEnumerator AutoHoldUp(float time)
    {
        yield return new WaitForSeconds(time);
        if (!holding && Input.GetButton("Hold"))    // get interrupt but still holding          ***Problem
        {
            StartHold();
        }
    }

    IEnumerator AutoHoldDown(float time)
    {
        yield return new WaitForSeconds(time);
        holding = false;
        anim.SetTrigger("attack");

        anim.SetTrigger("attackleg");
        StartCoroutine(AutoHoldUp(attackTime));
        //holded = true;
    }*/

    void Hold()
    {
        if (Input.GetButtonDown("Hold") && !blocking && !attacking)
        {
            StartHold();
        }
        if (Input.GetButtonUp("Hold"))
        {
            if (holding)
            {
                if (Time.time - countHold >= holdTime)
                {
                    countAttack = Time.time;
                    holding = false;
                    anim.SetTrigger("attack");
                    anim.SetTrigger("attackleg");
                }
                else
                {
                    waitingForAttack = true;
                    //countAttack = Time.time + (holdTime - (Time.time - countHold));
                }
            }
            else // !holding  = get interrupted
            {

            }
        }
    }

    void StartHold()
    {
        mouseScript.UpdateHoldDirect();
        holding = true;
        countHold = Time.time;
    }

    void WaitingForAttack()
    {
        if (waitingForAttack)
        {
            if (Time.time - countHold >= holdTime)
            {
                countAttack = Time.time;
                holding = false;
                anim.SetTrigger("attack");
                anim.SetTrigger("attackleg");

                waitingForAttack = false;
            }
        }
    }

    void CheckHoldOrAttack()    //place at last part
    {
        if (Time.time - countAttack >= attackTime)
            attacking = false;
        else
            attacking = true;

        if (!holding && Input.GetButton("Hold"))    // get interrupt but still holding
        {
            if (!blocking && !attacking)
            {
                StartHold();
            }
        }
    }

    void Block()
    {
        if (Input.GetButtonDown("Block"))
        {
            StartBlock();
        }
        if (Input.GetButtonUp("Block"))
        {
            if (blocking)
            {
                blocking = false;

            }
            else  // !blocking = get interrupted
            {
                
            }
        }
    }

    void StartBlock()
    {
        blocking = true;
        if (holding)
        {
            holding = false;
            waitingForAttack = false;
        }
    }

    public void TurnHand()
    {
        if (curRightHand)
        {

        }
    }

    public void TurnRightHand()
    {
        curRightHand = true;
        weapon.parent = rightHand;
        weapon.localPosition = RHSWeaponPos;
        weapon.localRotation = Quaternion.Euler(RHSWeaponRot);
    }

    public void TurnLeftHand()
    {
        curRightHand = false;
        weapon.parent = leftHand;
        weapon.localPosition = LHSWeaponPos;
        weapon.localRotation = Quaternion.Euler(LHSWeaponRot);
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
