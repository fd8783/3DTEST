Shader "Custom/test"
{
	Properties
	{
		_Texture("Texture2D",2D) = "white"{}
		_Color("TexColor", Color) = (1,1,1,1)
		_Transparent("Transparent", Range(0.0,1.0)) = 0.2
		_NoiseTexture("Noise",2D) = "white"{}
		_NoiseStrength("NoiseStrength", float) = 0.5
	}

	SubShader
	{
		GrabPass{
			"_GrabPassTexture"
		}


		Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
		

		ZWrite Off
		//Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			sampler2D _GrabPassTexture;
			float4 _GrabPassTexture_ST;

			#pragma	vertex vert
			#pragma	fragment frag

			#include "UnityCG.cginc"


			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				//float4 pos : SV_POSITION;

				float4 grabPos : TEXCOORD1;
			};
			
			struct v2f {
				//float4 vertex : POSITION;
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				
				float4 grabPos : TEXCOORD1;
			};

			sampler2D _Texture;
			float4 _Texture_ST; //i guess it's sth like ref, it is only used by TRANSFORM_TEX function
			float4 _Color;
			float _Transparent;

			sampler2D _NoiseTexture;
			float4 _NoiseTexture_ST;
			float _NoiseStrength;

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				//o.uv = TRANSFORM_TEX(v.uv, _Texture);

				//o.pos.x = abs(o.pos.x*1.5f);
				o.grabPos = ComputeGrabScreenPos(o.pos);
				//o.grabPos.x = abs(o.grabPos.x*1.5f);
				//o.uv = TRANSFORM_TEX(v.texcoord, _NoiseTexture);
				o.uv = v.uv;
				return o;
			}
				
			fixed4 frag(v2f i) : SV_Target{
				//fixed4 col = tex2D(_Texture, i.uv) * _Color;
				//col.a = _Transparent;
				//float2 offset = float2 ()

				float4 noiseOffSet = tex2D(_NoiseTexture, i.uv);

				i.grabPos.x -= noiseOffSet.z * _NoiseStrength;
				i.grabPos.y -= noiseOffSet.w *_NoiseStrength;

				fixed4 col = tex2Dproj(_GrabPassTexture, i.grabPos);

				return col*_Color;
			}

			ENDCG
		}
	}
}