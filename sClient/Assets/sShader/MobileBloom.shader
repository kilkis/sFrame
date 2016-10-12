
Shader "Hidden/MobileBloom" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Bloom ("Bloom (RGB)", 2D) = "black" {}
	}
	
	CGINCLUDE


		#pragma debug
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _Bloom;
		
        	#if SHADER_API_D3D9
		
		uniform half4 _MainTex_TexelSize;
		
			#endif

		uniform fixed4 _Parameter;
		uniform half4 _OffsetsA;
		uniform half4 _OffsetsB;
		
		#define ONE_MINUS_THRESHHOLD_TIMES_INTENSITY _Parameter.w
		#define THRESHHOLD _Parameter.z

		struct v2f_simple {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
        
        	#if SHADER_API_D3D9
        	
				half2 uv2 : TEXCOORD1;
			
			#endif
		};
		
		struct v2f_withMaxCoords {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			half2 uv20 : TEXCOORD1;
			half2 uv21 : TEXCOORD2;
			half2 uv22 : TEXCOORD3;
			half2 uv23 : TEXCOORD4;		
		};		

		struct v2f_withBlurCoords {
			half4 pos : SV_POSITION;
			half2 uv20 : TEXCOORD0;
			half2 uv21 : TEXCOORD1;
			half2 uv22 : TEXCOORD2;
			half2 uv23 : TEXCOORD3;

		};	
		
		v2f_simple vertBloom (appdata_img v)
		{
			v2f_simple o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			
        	o.uv = v.texcoord;		
        	
        	#if SHADER_API_D3D9
        	
        	o.uv2 = v.texcoord;			
        	        		        	
        		if (_MainTex_TexelSize.y < 0.0)
        			o.uv.y = 1.0 - o.uv.y;
        	
        	#endif
        	        	
			return o; 
		}

		v2f_withMaxCoords vertMax (appdata_img v)
		{
			v2f_withMaxCoords o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
        	o.uv20 = v.texcoord + _OffsetsA.xy;					
			o.uv21 = v.texcoord + _OffsetsA.zw;		
			o.uv22 = v.texcoord + _OffsetsB.xy;		
			o.uv23 = v.texcoord + _OffsetsB.zw;		
        	o.uv = v.texcoord;
			return o; 
		}			

		v2f_withBlurCoords vertBlur (appdata_img v)
		{
			v2f_withBlurCoords o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
        	o.uv20 = v.texcoord + _OffsetsA.xy;					
			o.uv21 = v.texcoord + _OffsetsA.zw;		
			o.uv22 = v.texcoord + _OffsetsB.xy;		
			o.uv23 = v.texcoord + _OffsetsB.zw;	
			return o; 
		}		
						
		fixed4 fragBloom ( v2f_simple i ) : COLOR
		{	
        	#if SHADER_API_D3D9
			
			fixed4 color = tex2D(_MainTex, i.uv);
			return color + tex2D(_Bloom, i.uv2);
			
			#else

			fixed4 color = tex2D(_MainTex, i.uv);
			return color + tex2D(_Bloom, i.uv);
						
			#endif
		} 
		
		fixed4 fragMax ( v2f_withMaxCoords i ) : COLOR
		{				
			fixed4 color = tex2D(_MainTex, i.uv.xy);
			color = max(color, tex2D (_MainTex, i.uv20));	
			color = max(color, tex2D (_MainTex, i.uv21));	
			color = max(color, tex2D (_MainTex, i.uv22));	
			color = max(color, tex2D (_MainTex, i.uv23));	
			return saturate(color - THRESHHOLD) * ONE_MINUS_THRESHHOLD_TIMES_INTENSITY;
		} 

		fixed4 fragBlur ( v2f_withBlurCoords i ) : COLOR
		{				
			fixed4 color = tex2D (_MainTex, i.uv20);
			color += tex2D (_MainTex, i.uv21);
			color += tex2D (_MainTex, i.uv22);
			color += tex2D (_MainTex, i.uv23);
			return color * 0.25;
		}
			
	ENDCG
	
	SubShader {
	  ZTest Off Cull Off ZWrite Off Blend Off
	  Fog { Mode off }  
	  
	// 0
	Pass {
	
		CGPROGRAM
		#pragma vertex vertBloom
		#pragma fragment fragBloom
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}
	// 1
	Pass { 
	
		CGPROGRAM
		
		#pragma vertex vertMax
		#pragma fragment fragMax
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}	
	// 2
	Pass {
	
		CGPROGRAM
		
		#pragma vertex vertBlur
		#pragma fragment fragBlur
		#pragma fragmentoption ARB_precision_hint_fastest 
		
		ENDCG
		 
		}			
	}
	FallBack Off
}
