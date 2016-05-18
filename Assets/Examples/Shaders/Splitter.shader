Shader "Hidden/TextureSplitter"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Prop ("property", Vector) = (0.0,0.0,0.5,1.0)
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _Prop;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;
				uv *= _Prop.zw;
				uv += _Prop.xy;
				fixed4 col = tex2D(_MainTex, uv);
				return col;
			}
			ENDCG
		}
	}
}
