Shader "Custom/Cavelight"
{
	Properties
	{
		_MainTex ("main texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { 

			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
	}
		Cull Off
		ZWrite Off
		ZTest Always
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _lightTex;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 light = tex2D(_lightTex, float2(i.uv.x, i.uv.y));
				light *= light.a;

				col.rgb *= col.a*(light+0.2);
				col += light*0.01;

				return col;
			}
			ENDCG
		}
	}
}
