Shader "Custom/SeeThrough"
{
	Properties
	{
		_MainTex ("main texture", 2D) = "white" {}
		_ColorST("color", Color) = (1,1,1,1)
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
			Stencil{
			Ref 4
			Comp equal
			Pass replace
		}
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
			fixed4 _ColorST;

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
				col.rgb *= col.a;
				float mask = step(0.1, col.a);
				return _ColorST *mask;
			}
			ENDCG
		}
	}
}
