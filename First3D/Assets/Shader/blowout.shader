Shader "Custom/blowout"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color1("Color1",Color) = (1,1,1,1)
		_Color2("Color2",Color) = (1,1,1,1)
		_DistortCenter("DistortCenter", Vector) = (0.5,0,0,0)
		_DistortPower("DistortPower",float) = 0.5

	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
				float2 screenPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color1;
			float4 _Color2;
			float4 col1;
			
			float2 _DistortCenter;
			float _DistortPower;
			float angle;

			uniform sampler2D globalCapTex;
			//uniform float4 globalCapTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.screenPos = ((o.vertex.xy / o.vertex.w) + 1) / 2;

				angle = cosh(float2(o.screenPos - _DistortCenter))*_DistortPower;
				o.screenPos.x = o.screenPos.x*cos(angle) - o.screenPos.y*sin(angle);

				o.screenPos.y = (1 - o.screenPos.y);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(globalCapTex, i.screenPos); //if we use i.uv, it use the ScreenCap for the whole object, i.e. distort in sphere

				return col;
			}
			ENDCG
		}
	}
}
