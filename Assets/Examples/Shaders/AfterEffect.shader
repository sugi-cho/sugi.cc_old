Shader "Hidden/AfterEffect"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Washi("washi tex",2D) = "gray" {}
		_WP("washi props(washiLoop,noiseLoop,noiseOffset)",Vector) = (2,1,0,0)
	}
		CGINCLUDE
#include "UnityCG.cginc"
#include "Assets/CGINC/ColorCollect.cginc"
#define TS _MainTex_TexelSize

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

		sampler2D _MainTex, _Washi, _NoiseTex, _PostBlendBG, _FullNoise, _BlurTex;
		half4 _AeHsvShift, _AeTone;
		half4 _MainTex_TexelSize, _WP;

		half4 frag(v2f i) : SV_Target
		{
			half4 c = tex2D(_MainTex, i.uv);
			half4 bg = tex2D(_PostBlendBG, i.uv);
			half4 fn = tex2D(_FullNoise, i.uv*0.1 + half2(0,_Time.x*0.3));
			half4 bt = tex2D(_BlurTex, i.uv);

			half3 hsv = rgb2hsv(c);
			hsv.y = blendScreen(hsv.y,saturate(bg.a)*0.3);
			c.rgb = hsv2rgb(hsv);
			c.rgb = blendScreen(c.rgb, bt.rgb*bt.rgb);

			half2 uv = i.uv * half2(TS.y*TS.z,1);
			half4 w = tex2D(_Washi, uv * _WP.x);

			half n = tex2D(_NoiseTex, uv * _WP.y).b;
			n = 1.5*pow(n,1.5);

			w = lerp(w,0.5,saturate(n))*(1 + fn*_WP.z);
			c.rgb = lerp(c.rgb,bg.rgb,saturate(bg.a)*0.5);
			c.rgb = lerp(c.rgb,2 * c.rgb*c.rgb,2 * length(ddx(c.rgb) + ddy(c.rgb)));
			half4 o = blendOverlay(c,w);
			c = lerp(c,o,sqrt(length(c.rgb))*_WP.w);

			half3 c1 = rgb2hsv(c.rgb);
			c1.xyz += _AeHsvShift.xyz;
			c1 = hsv2rgb(saturate(c1));
			c.rgb = lerp(c.rgb, c1, _AeHsvShift.w);
			c1 = _AeTone.x*pow(c.rgb + _AeTone.z,_AeTone.y);
			c.rgb = lerp(c.rgb,c1,_AeTone.w);

			c.rgb = GammaToLinearSpace(c.rgb);
			return c;
		}
			ENDCG
			SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

				Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 3.0

				ENDCG
			}
		}
}
