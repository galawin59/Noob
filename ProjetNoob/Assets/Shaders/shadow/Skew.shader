// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/skew"
{
	Properties
	{
		_MainTex ("main texture", 2D) = "white" {}
		//_skewX("skew x", Float) = 0.3
		_skewY("skew y", Float) = 0.3
		//_PersX("pers x", Range(-1.5,1.5)) = 1
		_PersY("pers y", Range(-1.5,1.5)) = 1
	
	//	_Angle("angle", Float) = 0.0
	}
	SubShader
	{
		Tags { 
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"DisableBatching" = "True"
	}
		LOD 100
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM


			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			//float _skewX;
			float _skewY;
			//float _PersX;
			float _PersY;
			//float4 _lightDir;
			//float _Angle;

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
				
			
				float rY = _skewY * _MainTex_TexelSize.w*0.001* v.uv.y;
			
				v.vertex.x += _PersY * rY;
				v.vertex.y += rY;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= col.a;
				return col;
			}
			ENDCG
		}
	}
}
