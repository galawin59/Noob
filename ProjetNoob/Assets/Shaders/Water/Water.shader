Shader "Custom/Water"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DetailTex("Detail Texture", 2D) = "white" {}
		_WaterColor("Water Color", Color) = (0.145,0.623,0.678,1.)
		_DetailColor("Detail Color", Color) = (1.,1.,1.,1.)
		_Alpha("alpha", Float) = 1.0

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
		Blend DstColor SrcColor
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
			sampler2D _DetailTex;
			fixed4 _WaterColor;
			fixed4 _DetailColor;
			float _Alpha;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)* _WaterColor;
		
				fixed4 detail = tex2D(_DetailTex, i.uv +_Time.y*0.05) ;
				fixed4 detailMask1 = tex2D(_DetailTex, float2(i.uv.x - _CosTime.x*0.1, i.uv.y - _SinTime.x*0.1));
				fixed4 detailMask2 = tex2D(_DetailTex, float2(i.uv.x + _CosTime.x*0.05, i.uv.y + _CosTime.x*0.05));
				fixed4 finalColor = lerp(col, (detail * detail.a)*_DetailColor*1.2 , detail * detailMask1*detailMask2*distance(i.uv*0.5+0.5, float2(0.5,0.5)));
				return finalColor;
			}
			ENDCG
		}
	}
}
