Shader "Custom/Explosion"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		
		_ExpBright("Brightest Fire Color", color) = (1, 1, 1, 1)
		_ExpDark("Darkest Fire Color", color) = (1, 1, 1, 1)
		_SmkBright("Brightest Smoke Color", color) = (1, 1, 1, 1)
		_SmkDark("Darkest Smoke Color", color) = (1, 1, 1, 1)
		
		_Progress("Progress", float) = 0
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

			sampler2D _MainTex;
			float _Progress;
			float4 _ExpBright, _ExpDark, _SmkBright, _SmkDark;

			fixed4 frag(v2f i) : SV_Target {
				float4 color = tex2D(_MainTex, i.uv);
				float stage2 = _Progress * 2;
				float stage1 = stage2 - 1;
				float4 e = float4(0, 0, 0, 0);
				
				if (color.a > 0) {
					/**/ if (color.r * color.g > _Progress && color.r / color.g > _Progress) e = lerp(_ExpBright, _ExpDark, _Progress);
					else if (color.g > _Progress) e = lerp(_SmkBright, _SmkDark, stage2);
				}

				float m = color.g;
				return float4(e.r * m, e.g * m, e.b * m, e.a);
			}

			ENDCG
		}
	}
}
