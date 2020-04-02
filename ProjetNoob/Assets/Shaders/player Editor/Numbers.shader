Shader "Custom/numbers"
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
			"CanUseSpriteAtlas" = "True"
	}
		
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
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); //Projection du modèle 3D, cette ligne est obligatoire
				o.uv = v.uv; //UV de la texture
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, float2(i.uv.x, _Time.y*0.15 + -sin(i.uv.x*1.5 + 0.85)*(1.1 - i.uv.y))*1.5);
				
				float d = distance(i.uv.xy, float2(0.5, 0.0))*0.8;
				col *= col.a*i.color.a;

				col *= lerp(0, 8,d - 0.2);
			
				return col;
			}
			ENDCG
		}
	}
}
