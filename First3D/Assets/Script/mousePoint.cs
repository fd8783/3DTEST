using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class mousePoint : MonoBehaviour {

    public bool pointUp, pointDown,pointRight, pointLeft;

    private Animator anim;

    private Vector2 mousePreMove, mouseCurMove, mouseMoveDis;

    // Use this for initialization
    void Awake () {
        mousePreMove = Input.mousePosition;

        anim = GetComponentInChildren<Animator>();
        pointUp = true;
        anim.SetBool("mouseUp", pointUp);
	}
	
	// Update is called once per frame
	void Update () {
        MouseAxisUpdate();

    }

    void MouseAxisUpdate()
    {
        mouseCurMove = Input.mousePosition;
        mouseMoveDis = mouseCurMove - mousePreMove;

        if (mouseMoveDis != Vector2.zero)
        {
            Debug.Log(mouseMoveDis);
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
}
