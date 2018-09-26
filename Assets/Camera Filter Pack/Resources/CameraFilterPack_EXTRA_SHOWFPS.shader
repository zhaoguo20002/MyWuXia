// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

///////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 ///
///////////////////////////////////////////

Shader "CameraFilterPack/EXTRA_SHOWFPS" { 
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
uniform float _Value2;
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


// SHADER NEED TO BE IMPROVED

// inline float mod(float x,float modu) 
// {
// return x - floor(x * (1.0 / modu)) * modu;
// }

int D(float2 p, float n) 
{
//   int i=int(p.y), b=int(pow(2.,floor(30.-p.x-n*3.)));
//   i = p.x<0.||p.x>3.? 0:
//   i==5? 972980223: i==4? 690407533: i==3? 704642687: i==2? 696556137:i==1? 972881535: 0;
// 	return i/b-2*(i/b/2);
	return 0;
}

fixed4 frag (v2f i) : COLOR
{
//	float2 uv=i.texcoord;
//   fixed4 fps=float4(0,0,0,0);
//    uv*=512;
//    uv/=_Value;
//	  float c=1e3;
//    uv.x*=2;
//    for (int n=0; n<4; n++)
//    { 
//     if ((uv.x-=4.)<3.) { float h=saturate(D(uv,mod(floor(_Value2/c),10.)));
//     if (_Value2>45) fps += float4(0.,h,0.,1.0);
//     else
//     if (_Value2>30) fps += float4(h,h,0.,1.0);
//     else
//     fps += float4(h,0.,0.,1.0);
//     } 
//     c*=.1;
//    }
//    fixed4 txt=tex2D(_MainTex,i.texcoord);
//    txt+=fps;
    return float4(0,0,0,0);
}
ENDCG
}
}
}
