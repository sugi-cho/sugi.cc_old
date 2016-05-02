Shader "Unlit/BillboardWithDepth"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Amp("depth amp", Float) = 1
		_Threshold("depth threshold", Float) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
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
					float4 vPos : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float _Threshold,_Amp;

				v2f vert(appdata v)
				{
					v.vertex.xy -= v.uv - 0.5;
					float4 vPos = mul(UNITY_MATRIX_MV, v.vertex);
					vPos.xy += v.uv - 0.5;

					v2f o;

					o.vertex = mul(UNITY_MATRIX_P, vPos);
					o.vPos = vPos;
					o.uv = v.uv;
					return o;
				}

				half4 frag(v2f i, out float depth : SV_Depth) : SV_Target
				{
					half4 col = tex2D(_MainTex, i.uv);
					if (col.a < -_Threshold)
						discard;
					col.a *= _Amp;
					half4 vPos = i.vPos + half4(0,0,col.a,0);
					half4 pPos = mul(UNITY_MATRIX_P, vPos);

					depth = pPos.z / pPos.w;
					clip(depth);

					return col;
				}
				ENDCG
			}
		}
}
