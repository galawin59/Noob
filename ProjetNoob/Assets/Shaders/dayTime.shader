Shader "Custom/daytime"
{
	Properties
	{
		_MainTex ("main texture", 2D) = "white" {}
		_MorningColor("morning color", Color) = (1.0,1.0,1.0,1.0)
		_AfternoonColor("afternoon color", Color) = (1.0,1.0,1.0,1.0)
		_InGameTime("in game time", Range(-0.5, 0.5)) = 0.0
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
			sampler2D _lightTex;
			fixed4 _MorningColor;
			fixed4 _AfternoonColor;
			float _InGameTime;
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
				float positiveTime = clamp(abs(_InGameTime),0.0,0.5);

				fixed4 col = tex2D(_MainTex, float2(i.uv.x, i.uv.y));
				fixed4 light = tex2D(_lightTex, float2(i.uv.x, i.uv.y));
				fixed4 flight = light;
				light *= light.a;
				light *= positiveTime;
				light = 1- light;
				
				col *= col.a;
				if (_InGameTime > 0)
				{
					light = fixed4(1, 1, 1, 1);
					flight *= 0;
				}
				col.rgb += clamp(_InGameTime, -0.5,0.0)*light;
				col.rgb += lerp(_AfternoonColor.rgb*positiveTime, _MorningColor.rgb*positiveTime, 0.5 + _InGameTime)*light;
				
				col += flight * positiveTime*0.5;//*lightColor;
				return col;
			}
			ENDCG
		}
	}
}
