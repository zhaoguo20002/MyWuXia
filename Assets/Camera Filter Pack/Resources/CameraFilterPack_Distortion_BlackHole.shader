// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

////////////////////////////////////////////////////////////////////////////////////
//  CameraFilterPack v2.0 - by VETASOFT 2015 //////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////

Shader "CameraFilterPack/Distortion_BlackHole" {
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_TimeX ("Time", Range(0.0, 1.0)) = 1.0
		_Distortion ("_Distortion", Range(0.0, 1.0)) = 0.3
		_Distortion2 ("_Distortion", Range(0.0, 1.0)) = 0.3
		_ScreenResolution ("_ScreenResolution", Vector) = (0.,0.,0.,0.)
		_PositionX ("_PositionX", Range(-1.0, 1.0)) = 1.5
		_PositionY ("_PositionY", Range(-1.0, 1.0)) = 30.0
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
			uniform float _Distortion;
			uniform float _Distortion2;
			uniform float4 _ScreenResolution;
			uniform float _PositionX;
			uniform float _PositionY;
			
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

			float4 frag (v2f i) : COLOR
			{
				float2 uv = i.texcoord.xy;

				float2 center = float2((0.5 + _PositionX / 2) * _ScreenResolution.x , (0.5 - _PositionY / 2) * _ScreenResolution.y);
				float2 warp = normalize(center.xy - (i.texcoord.xy * _ScreenResolution.xy)) * pow(distance(center.xy, (i.texcoord.xy * _ScreenResolution.xy)), -2.0) * _Distortion2* _Distortion2;
				warp.y = -warp.y;
				uv = uv + warp;
				
				float light = clamp(0.1*distance(center.xy, (i.texcoord.xy * _ScreenResolution.xy)) - _Distortion, 0.0, 1.0);
				
				return tex2D(_MainTex, uv) * light;
	
			}
			
			ENDCG
		}
		
	}
}