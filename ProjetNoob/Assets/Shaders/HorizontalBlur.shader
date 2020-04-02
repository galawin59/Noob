Shader "Custom/HorizontalBlur"
{
	Properties
	{
		_MainTex ("main texture", 2D) = "white" {}
		_NbSampler("sampler", Float) = 4
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float _NbSampler;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); //Projection du modèle 3D, cette ligne est obligatoire
				o.uv = v.uv; //UV de la texture
			
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float uvX = i.uv.x;
				fixed4 col = fixed4(0,0,0,0);
				for (int j = -_NbSampler; j < _NbSampler; j++)
				{
					col += tex2D(_MainTex, float2(i.uv.x, i.uv.y + j * 0.01))*(1 - abs(j) / _NbSampler);
				}
				
				col *= col.a;
				col /= _NbSampler*2;
				return col;
			}
			ENDCG
		}
	}
}
