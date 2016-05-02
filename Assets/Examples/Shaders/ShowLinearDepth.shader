Shader "Hidden/ShowLinearDepth"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ZD("zero point", Float) = 2
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

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
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				sampler2D _CameraDepthTexture;
				float _ZD;

				half4 frag(v2f i) : SV_Target
				{
					half4 c = tex2D(_MainTex, i.uv);
					float d = tex2D(_CameraDepthTexture, i.uv).r;

					c.a = _ZD - LinearEyeDepth(d);
					return c;
				}
				ENDCG
			}
		}
}
