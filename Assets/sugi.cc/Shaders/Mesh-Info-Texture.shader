Shader "Generator/mesh Info texture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100 ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 vPos : TEXCOORD1;
				float3 vNorm : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			struct pOut
			{
				float4 VertexPosition : SV_Target0;
				float4 VertexNormal : SV_Target1;
			};
			
			v2f vert (appdata v)
			{
				float3 vPos = v.vertex.xyz;
				float3 vNorm = v.normal;
				v.vertex.xyz = float3(v.uv,0);
				
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex = float4(float2(v.uv.x,1-v.uv.y)*2.0-1.0,0.0,1.0);
				o.uv = v.uv;
				o.vPos = vPos;
				o.vNorm = vNorm;
				return o;
			}
			
			pOut frag (v2f i)
			{
				pOut o;
				o.VertexPosition = float4(i.vPos,1.0);
				o.VertexNormal = float4(i.vNorm,1.0);
				return o;
			}
			ENDCG
		}
	}
}
