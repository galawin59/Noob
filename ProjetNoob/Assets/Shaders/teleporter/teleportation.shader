// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/teleportation"
{
	Properties
	{
		_MainTex ("main texture", 2D) = "white" {}
		_Xpos("X in world position", Float) = 0
		_Ypos("Y in world position", Range(0,1.5)) = 0
		_color("teleportation color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { 
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : color;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float2 currentWorldPos : POSITION1;
				float2 currentUvPos : POSITION2;
				fixed4 color : color;
			};

			sampler2D _MainTex;
			fixed4 _color;
			float _Xpos;
			float _Ypos;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.currentUvPos = mul(unity_ObjectToWorld, v.vertex.xy);
				o.currentWorldPos = mul(unity_ObjectToWorld, float2(_Xpos,_Ypos));
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{	
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= col.a;
				if (i.currentUvPos.y + 0.5 > i.currentWorldPos.y)
				{
					col = fixed4(0, 0, 0, 0);
				}
				else
				{
					float dist = clamp(distance(i.currentWorldPos.y, i.uv.y)*2., 0, 1);
					col *= dist;
					float diameter = (1 - dist)*col.a*5.0;
					col = lerp(col,_color * diameter,diameter);
				}
					
				return col * i.color.a;
			}
			ENDCG
		}
	}
}
