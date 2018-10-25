Shader "Custom/Normal2D"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Normal("Normal", 2D) = "white" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f {
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
			
			v2f vert(appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex, _Normal;
			float _Up, _Dn, _Rt, _Lt;
			//up, down, left, right
			
			fixed4 frag (v2f i) : SV_Target {
				float4 color = tex2D(_MainTex, i.uv);
				float4 normal = tex2D(_Normal, i.uv);
				float nX = normal.a * 2 - 1;
				float nY = normal.g * 2 - 1;
				bool Rt = (nX > 0), Lt = !Rt;
				bool Up = (nY > 0), Dn = !Up;

				float dt = 0; //dot
				/**/ if (Rt && Up) dt = dot(float2(nX, nY), float2(_Rt, _Up));
				else if (Rt && Dn) dt = dot(float2(nX, nY), float2(_Rt, _Dn));
				else if (Lt && Up) dt = dot(float2(nX, nY), float2(_Lt, _Up));
				else if (Lt && Dn) dt = dot(float2(nX, nY), float2(_Lt, _Dn));

				dt = (dt + 1) / 2; //inverse of nX and nY
				return float4(color.r * dt, color.g * dt, color.b * dt, color.a);
			}
			
			ENDCG
		}
	}
}
