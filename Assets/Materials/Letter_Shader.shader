Shader "Custom/Letter"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TextColor("Text Color", color) = (1, 0, 0, 1)
		_BorderColor("Border Color", color) = (0, 0, 0, 1)
		_HighlightColor("Highlighted Border Color", color) = (1, 1, 1, 1)
		[PerRendererData] _Highlighted("Highlighted", int) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
		}

		Pass
		{
			ZWrite Off
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

			sampler2D _MainTex;
			float4 _TextColor, _BorderColor, _HighlightColor;
			int _Highlighted;

			fixed4 frag(v2f i) : SV_Target {
				float4 color = tex2D(_MainTex, i.uv);

				float a = color.a;
				if (color.r == 0) {
					if (_Highlighted == 1) color = _HighlightColor;
					else color = _BorderColor;
				}
				else color = _TextColor;

				return float4(color.r, color.g, color.b, a);
			}

			ENDCG
		}
	}
}
