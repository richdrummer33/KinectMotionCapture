
// Toony Colors Pro+Mobile 2
// (c) 2014-2016 Jean Moreno


Shader "CC/Toony Colors Pro 2/Dissolve"
{
	Properties
	{
		//TOONY COLORS
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_HColor ("Highlight Color", Color) = (0.6,0.6,0.6,1.0)
		_SColor ("Shadow Color", Color) = (0.4,0.4,0.4,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB) Spec/Refl Mask (A) ", 2D) = "white" {}
		
		//TOONY COLORS RAMP
		_Ramp ("#RAMPT# Toon Ramp (RGB)", 2D) = "gray" {}
		_RampThreshold ("#RAMPF# Ramp Threshold", Range(0,1)) = 0.5
		_RampSmooth ("#RAMPF# Ramp Smoothing", Range(0.01,1)) = 0.1
		
		//BUMP
		_BumpMap ("#NORM# Normal map (RGB)", 2D) = "bump" {}

		// DISSOLVE
		_NoiseTex("Dissolve Noise", 2D) = "white"{}
		_NScale("Noise Scale", Range(0, 10)) = 1
		_DisAmount("Noise Texture Opacity", Range(0.01, 1)) = 0
		[Toggle(ALPHA)] _ALPHA("No Shadows on Transparent", Float) = 1

		// Masking
		_ID("Mask ID", Int) = 0 // for masking
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue" = "Geometry+2"}
		LOD 200

			// This means the material wont be shown if viewed through a mask
			// Comp notequal if want the reverse to be true
		  Stencil {
				Ref[_ID]
				Comp equal
			}
			// transparency
			Blend SrcAlpha OneMinusSrcAlpha 
		
		CGPROGRAM
		
		#include "Include/TCP2_Include.cginc"

		
		#pragma surface surf ToonyColors alphatest:_ALPHA addshadow// transparency
		#pragma target 3.0
		#pragma glsl
		
		#pragma shader_feature TCP2_DISABLE_WRAPPED_LIGHT
		#pragma shader_feature TCP2_RAMPTEXT
		#pragma shader_feature TCP2_BUMP
		
		//================================================================
		// VARIABLES
		
		float4 _Color;
		sampler2D _MainTex;
		// Dissolve
		uniform float4 _Position; // receives position in x,y,z and radius in w
		uniform float _ArrayLength; // for multiple positions
		uniform float4 _Positions[25]; // for multiple positions
		float _Radius;
		sampler2D _NoiseTex;//
		float _DisAmount, _NScale;//
		
	#if TCP2_BUMP
		sampler2D _BumpMap;
	#endif
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
			float3 worldPos;// built in value to use the world space position
			float3 worldNormal;
	#if TCP2_BUMP
			half2 uv_BumpMap : TEXCOORD1;
	#endif
		};
		
		//================================================================
		// SURFACE FUNCTION
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);

			// triplanar noise
			float3 blendNormal = saturate(pow(IN.worldNormal * 1.4, 4));
			half4 nSide1 = tex2D(_NoiseTex, (IN.worldPos.xy + _Time.x) * _NScale);
			half4 nSide2 = tex2D(_NoiseTex, (IN.worldPos.xz + _Time.x) * _NScale);
			half4 nTop = tex2D(_NoiseTex, (IN.worldPos.yz + _Time.x) * _NScale);

			float3 noisetexture = nSide1;
			noisetexture = lerp(noisetexture, nTop, blendNormal.x);
			noisetexture = lerp(noisetexture, nSide2, blendNormal.y);

			// code for multiple influencers
			float3 sphereRNoise;
			for (int i = 0; i < 25; i++) {
				_Radius = _Positions[i].w;
				float3 dis = distance(_Positions[i], IN.worldPos);
				float3 sphereR = 1 - saturate(dis / _Radius);
				sphereR *= noisetexture;
				sphereRNoise += (sphereR);
			}

			c.a = step(_DisAmount, sphereRNoise);
			o.Albedo = c.rgb * _Color.rgb;
			//o.Alpha = c.a * _Color.a;
			o.Alpha = c.a;
			
	#if TCP2_BUMP
			//Normal map
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
	#endif
		}
		
		ENDCG
		
	}
	
	Fallback "Diffuse"
	CustomEditor "TCP2_MaterialInspector"
}