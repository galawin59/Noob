Shader "Custom/teleporterLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_radius("circle radius" , Float) = 4.0
		_rangeRadius("circle radius" , Range(0,1)) = 1.0
		_color("light color" , Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader
	{
		Tags {"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"}

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float _radius;
			float _rangeRadius;
			fixed4 _color;
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
				fixed4 col = tex2D(_MainTex, i.uv);
				col *= col.a;
				
				float t = (cos(_Time.y*0.5) + 1)*0.5;
				
			
				/*float d1 = distance(float2(0.5, 0.5), float2(t, t));
				float d2 = distance(float2(0.5, 0.5), float2(t, t)*0.5);
				col *= d1;*/
				float dist = distance(float2(0.53, 0.55), i.uv.xy);
				float Dot = dot(dist, _radius*_rangeRadius);
				float circle = smoothstep(0.0, 1.0, Dot);
				float circle2 = smoothstep(0.7, 0.8, Dot);
				col = (1 - circle)*col;
				col += (circle2)*col*10;
				return col* _color;
			}
			ENDCG
		}
	}
}
