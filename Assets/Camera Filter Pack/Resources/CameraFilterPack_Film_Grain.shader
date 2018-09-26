// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////


Shader "CameraFilterPack/Film_Grain" { 
Properties 
{
_MainTex ("Base (RGB)", 2D) = "white" {}
_TimeX ("Time", Range(0.0, 1.0)) = 1.0
_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
}
SubShader
{
Pass
{
ZTest Always
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#pragma target 3.0
#pragma glsl
#include "UnityCG.cginc"
uniform sampler2D _MainTex;
uniform float _TimeX;
uniform float _Value;
uniform float4 _ScreenResolution;
struct appdata_t
{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};
struct v2f
{
half2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
fixed4 color    : COLOR;
};
v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}

inline float modx(float x,float modu) {
  return x - floor(x * (1.0 / modu)) * modu;
}  

inline float2 modx(float2 x,float2 modu) {
  return x - floor(x * (1.0 / modu)) * modu;
} 
inline float3 modx(float3 x,float3 modu) {
  return x - floor(x * (1.0 / modu)) * modu;
} 
  
inline float4 modx(float4 x,float4 modu) {
  return x - floor(x * (1.0 / modu)) * modu;
} 

float4 frag (v2f i) : COLOR
{
float2 uv = i.texcoord.xy;
float4 color = tex2D(_MainTex, uv);
float strength = _Value;
float x = (uv.x + 4.0) * (uv.y + 4.0) * (_TimeX * 10.0);
float g=modx((modx(x, 13.0) + 1.0) * (modx(x, 123.0) + 1.0), 0.01)-0.005;
float4 grain = float4(g,g,g,g) * strength;
return  color + grain;
}
ENDCG
}
}
}
