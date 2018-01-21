Shader "Custom/test"
{
	Properties
	{
		_Texture("Texture2D",2D) = "white"{}
		_Color("TexColor", Color) = (1,1,1,1)
	}

	SubShader
	{
		//Tags {"RenderType" = "Opaque"}
		LOD 200

		Pass
		{
			CGPROGRAM
			#pragma
				



			CGEND
		}

	}



}