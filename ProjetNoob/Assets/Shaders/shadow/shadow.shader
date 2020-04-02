Shader "Custom/shadow"
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
			sampler2D _shadowTex;
		
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
				fixed4 col = tex2D(_MainTex,  i.uv.xy);
				fixed4 shdw = tex2D(_shadowTex, i.uv.xy*0.8 + 0.1);
				if(shdw.a!=0.0f)
					shdw = lerp(fixed4(1.0, 1.0, 1.0, 1.0),fixed4(0.6, 0.6, 0.6, 1.0), shdw.a);
				else shdw = fixed4(1.0, 1.0, 1.0, 1.0);
				
				col *= col.a;
				
				return col*shdw;
			}
			ENDCG
		}
	}
}
