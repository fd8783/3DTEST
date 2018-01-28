using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] //** run in edit mode
public class ScreenCap : MonoBehaviour {

	private Camera cam;

    private string _globalCapTex = "globalCapTex";

	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void OnEnable ()
    {
        cam = GetComponent<Camera>();
        Debug.Log(333);
        if (cam.targetTexture != null)
        {
            RenderTexture temp = cam.targetTexture;
            cam.targetTexture = null;
            DestroyImmediate(temp);
        }


		cam.targetTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 16);
                                                    //16 ,depth,	Number of bits in depth buffer (0, 16 or 24). Note that only 24 bit depth has stencil buffer.
        cam.targetTexture.filterMode = FilterMode.Bilinear;

        Shader.SetGlobalTexture(_globalCapTex, cam.targetTexture);
    }
}
