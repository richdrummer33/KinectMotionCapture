﻿Shader "CC/CCDissolveShader_Alpha_Multiple" {
    Properties {
        _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
		[HDR]_Emission("Emission", Color) = (0,0,0,0)
        _Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
        _NoiseTex("Dissolve Noise", 2D) = "white"{} 
        _NScale ("Noise Scale", Range(0, 10)) = 1 
        _DisAmount("Noise Texture Opacity", Range(0.01, 1)) = 0 
       // _Radius("Radius", Range(0, 20)) = 0 // set in Shader controller script
        _DisLineWidth("Line Width", Range(0, 2)) = 0 
        _DisLineColor("Line Tint", Color) = (1,1,1,1)           
        [Toggle(ALPHA)] _ALPHA("No Shadows on Transparent", Float) = 0
		_ID("Mask ID", Int) = 1 // for masking
          
    }
 
        SubShader{
            Tags { "RenderType" = "Opaque" "Queue" = "Geometry+2"}
            LOD 200

			// This means the material wont be shown if viewed through a mask
			// Comp notequal if want the reverse to be true
		  Stencil {
				Ref[_ID]
				Comp equal
			}
           
        Blend SrcAlpha OneMinusSrcAlpha // transparency
CGPROGRAM
 
#pragma shader_feature LIGHTMAP
#pragma surface surf ToonRamp alphatest:_ALPHA addshadow// transparency
 
sampler2D _Ramp;
float4 _Emission;
 
// custom lighting function that uses a texture ramp based
// on angle between light direction and normal
#pragma lighting ToonRamp exclude_path:prepass
inline half4 LightingToonRamp (SurfaceOutput s, half3 lightDir, half atten)
{
    #ifndef USING_DIRECTIONAL_LIGHT
    lightDir = normalize(lightDir);
    #endif
   
    half d = dot (s.Normal, lightDir)*0.5 + 0.5;
    half3 ramp = tex2D (_Ramp, float2(d,d)).rgb;
	float4 emission = _Emission;
   
    half4 c;
    c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2)* emission;
    //c.a = 0; we don't want the alpha
    c.a = s.Alpha; // use the alpha of the surface output
    return c;
}
uniform float4 _Position; // receives position in x,y,z and radius in w
uniform float _ArrayLength; // for multiple positions
uniform float4 _Positions[100]; // for multiple positions
float _Radius;
 
sampler2D _MainTex;
float4 _Color;
sampler2D _NoiseTex;//
float _DisAmount, _NScale;//
float _DisLineWidth;//
float4 _DisLineColor;//
 
 
struct Input {
    float2 uv_MainTex : TEXCOORD0;
    float3 worldPos;// built in value to use the world space position
	float3 worldNormal;
   
};
 
void surf (Input IN, inout SurfaceOutput o) {
    half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

// triplanar noise
	 float3 blendNormal = saturate(pow(IN.worldNormal * 1.4,4));
    half4 nSide1 = tex2D(_NoiseTex, (IN.worldPos.xy + _Time.x) * _NScale); 
	half4 nSide2 = tex2D(_NoiseTex, (IN.worldPos.xz + _Time.x) * _NScale);
	half4 nTop = tex2D(_NoiseTex, (IN.worldPos.yz + _Time.x) * _NScale);

	float3 noisetexture = nSide1;
    noisetexture = lerp(noisetexture, nTop, blendNormal.x);
    noisetexture = lerp(noisetexture, nSide2, blendNormal.y);

	// distance influencer position to world position and sphere radius
	//code for single influence
	/*float3 dis = distance(_Position, IN.worldPos);
	float3 sphereR = 1 - saturate(dis / _Radius);
	float3 sphereRNoise = noisetexture * sphereR.r;
	float3 DissolveLineIn = step(sphereRNoise- _DisLineWidth, _DisAmount);
    float3 NoDissolve = float3(1, 1, 1) - DissolveLineIn ;
    c.rgb = (DissolveLineIn * _DisLineColor) + (NoDissolve * c.rgb);
    o.Emission =  (DissolveLineIn * _DisLineColor) * 2; */ 

		// code for multiple influencers
	float3 sphereRNoise;
	for  (int i = 0; i < 100; i++){
		_Radius = _Positions[i].w;
		float3 dis =  distance(_Positions[i], IN.worldPos);
		float3 sphereR = 1-  saturate(dis / _Radius);
		sphereR *= noisetexture;
		sphereRNoise += (sphereR);
	}

    c.a = step(_DisAmount, sphereRNoise);
    o.Albedo = c.rgb;
    o.Alpha = c.a;


  
}
ENDCG
 
    }
 
    Fallback "Diffuse"
}