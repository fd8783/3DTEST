using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class mousePoint : MonoBehaviour {

    public bool pointUp, pointDown,pointRight, pointLeft;

    public bool holdUp, holdDown, holdRight, holdLeft;

    private movementCtrl moveScript;
    private Animator anim;

    private Vector2 mousePreMove, mouseCurMove, mouseMoveDis;

    // Use this for initialization
    void Awake () {
        mousePreMove = Input.mousePosition;

        moveScript = transform.parent.GetComponent<movementCtrl>();
        anim = GetComponent<Animator>();
        pointUp = true;
        anim.SetBool("mouseUp", pointUp);
	}
	
	// Update is called once per frame
	void Update ()
    {
        MouseAxisUpdate();

    }

    void MouseAxisUpdate()
    {
        mouseCurMove = Input.mousePosition;
        mouseMoveDis = mouseCurMove - mousePreMove;

        if (mouseMoveDis != Vector2.zero)
        {
            //Debug.Log(mouseMoveDis);
            if (Mathf.Abs(mouseMoveDis.x) >= Mathf.Abs(mouseMoveDis.y))
            {
                pointUp = false;
                pointDown = false;
                
                if (mouseMoveDis.x > 0)
                {
                    pointRight = true;
                    pointLeft = false;
                }
                else
                {
                    pointRight = false;
                    pointLeft = true;
                }
            }
            else
            {
                pointRight = false;
                pointLeft = false;

                if (mouseMoveDis.y > 0)
                {
                    pointUp = true;
                    pointDown = false;
                }
                else
                {
                    pointUp = false;
                    pointDown = true;
                }
            }
            anim.SetBool("mouseUp", pointUp);
            anim.SetBool("mouseDown", pointDown);
            anim.SetBool("mouseRight", pointRight);
            anim.SetBool("mouseLeft", pointLeft);
        }
        mousePreMove = mouseCurMove;
    }

    /*public void UpdateHoldDirect(string direct)   //U = Up, D = Down, R = Right, L = Left         // * animation event in the unity model setting don't sent char
    {
        holdUp = false;
        holdDown = false;
        holdRight = false;
        holdLeft = false;
        switch (direct)
        {
            case "U":
                holdUp = true;
                break;
            case "D":
                holdDown = true;
                break;
            case "R":
                holdRight = true;
                break;
            case "L":
                holdLeft = true;
                break;
            
            default:
                break;
        }
        anim.SetBool("holdUp", holdUp);
        anim.SetBool("holdDown", holdDown);
        anim.SetBool("holdRight", holdRight);
        anim.SetBool("holdLeft", holdLeft);
        Debug.Log("DfFS");
    }
    */

        //annotate* now UpdateHoldDirect is called by animator for every hold direct changing, somehow it call all function have same name so I need to comment zone this one
    
    public void UpdateHoldDirect()  
    {
        holdUp = pointUp;
        holdDown = pointDown;
        holdRight = pointRight;
        holdLeft = pointLeft;
        anim.SetBool("holdUp", holdUp);
        anim.SetBool("holdDown", holdDown);
        anim.SetBool("holdRight", holdRight);
        anim.SetBool("holdLeft", holdLeft);
        Debug.Log("DFS");
    }
}
