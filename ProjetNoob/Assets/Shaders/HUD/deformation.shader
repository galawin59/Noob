Shader "Custom/fogDeformation"
{
	Properties
	{
		_MainTex ("main texture", 2D) = "white" {}
		_DeformTex("deform texture", 2D) = "white" {}
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
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			sampler2D _DeformTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 deform = tex2D(_DeformTex, float2(i.uv.x + _CosTime.x*0.8,i.uv.y + _SinTime.x*0.8)*0.5+0.5)*1.2;
				fixed4 col = tex2D(_MainTex, float2(i.uv.x + deform.r*0.01, i.uv.y + deform.g*0.01));
				col *= col.a*i.color.a;

				return col;
			}
			ENDCG
		}
	}
}
