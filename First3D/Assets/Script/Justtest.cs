using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Justtest : MonoBehaviour {

    int[] A = { 10 };

	// Use this for initialization
	void Start ()
    {
        Debug.Log(A[0]);
        Try(A);
        Debug.Log(A[0]);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Try(int[] a)
    {
        int[] b = { 1 };
        a = b;
        a[0] = 2;
    }
}
