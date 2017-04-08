// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/FractalNoise"
{
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _NoiseTex;

			half frag (v2f i) : SV_Target
			{
				float scale = 1.0;
				float amp = 0.5;
				half n = 0;

				for(int itr = 0; itr<5; itr++){
					n += amp * tex2D(_NoiseTex, i.uv*scale);
					scale *= 2.0;
					amp *= 0.5;
				}

				return n;
			}
			ENDCG
		}
	}
}
