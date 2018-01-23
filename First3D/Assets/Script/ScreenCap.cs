using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCap : MonoBehaviour {

	private Camera cam;

	// Use this for initialization
	void Awake () {
		cam = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void OnEnable () {
		Debug.Log(333);

		cam.targetTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
		cam.targetTexture.filterMode = FilterMode.Bilinear;
	}
}
