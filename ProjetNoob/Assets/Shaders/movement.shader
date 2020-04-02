Shader "Custom/mvt"
{
	Properties
	{
		_MainTex ("main texture", 2D) = "white" {}
		_OtherTex("other texture", 2D) = "white" {}
		_Color("color", Color) = (1.0,1.0,1.0,1.0)
		_X("x movement", Float) = 0 
		_Y("y movement", Float) = 0 
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
			sampler2D _OtherTex;
			fixed4 _Color;
			float _X;
			float _Y;
			float2 _camPos;

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
				float2 worldPos = (_WorldSpaceCameraPos.xy*0.5) / unity_OrthoParams.xy;
				fixed4 other = tex2D(_OtherTex, (float2(i.uv.x + _Time.x*_X, i.uv.y + _Time.x*_Y) + worldPos) *0.5);
				fixed4 col = tex2D(_MainTex, i.uv.xy);
				col *= col.a*i.color.a;
				col = lerp(col, col *float4(_Color.rgb,1.0), other);
				return col;
			}
			ENDCG
		}
	}
}
