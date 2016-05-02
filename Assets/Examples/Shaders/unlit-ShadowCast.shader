Shader "Unlit/unlit-ShadowCast"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
	CGINCLUDE
		#include "UnityCG.cginc"

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

		struct v2f
		{
			float4 hpos : TEXCOORD0;
			float2 uv : TEXCOORD1;
			float4 pos : SV_POSITION;
		};

		sampler2D _MainTex;
		float4 _MainTex_ST;

		v2f vert(appdata v)
		{
			v.vertex.y += 0.5;
			v.vertex.xz *= v.vertex.y*v.vertex.y*2;
			v.vertex.y -= 0.5;
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.hpos = o.pos;
			o.uv = TRANSFORM_TEX(v.uv, _MainTex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			// sample the texture
			fixed4 col = tex2D(_MainTex, i.uv);
			return col;
		}
		
		v2f vert_shadow(appdata v){
			v.vertex.y += 0.5;
			v.vertex.xz *= v.vertex.y*v.vertex.y*2;
			v.vertex.y -= 0.5;
			v2f o;
			float4 opos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = opos;
			opos.z += saturate(unity_LightShadowBias.x/opos.w);
			float clamped = max(opos.z, opos.w * 0);
			opos.z = lerp(opos.z, clamped, unity_LightShadowBias.y);
			o.hpos = opos; 
			o.uv = v.uv;
			return o;
		}
		
		fixed4 frag_shadow(v2f i) : SV_Target{
			SHADOW_CASTER_FRAGMENT(i)
		}
	ENDCG

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			ENDCG
		}
		
		Pass
		{
			Name "ShadowCaster"
			Tags{"LightMode" = "ShadowCaster"}
			ZWrite On ZTest LEqual
			
			CGPROGRAM
			#pragma vertex vert_shadow
			#pragma fragment frag_shadow

			ENDCG
		}
	}
}
