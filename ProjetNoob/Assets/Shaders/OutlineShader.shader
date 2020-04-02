// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/OutlineShader" {
Properties {
	_MainTex ("mainTexture", 2D) = "" {}
	_OutlineColor("OutlineColor", Color) = (1,1,1,1)
	_SecondOutlineColor("SecondOutlineColor", Color) = (1,1,1,1)
}
SubShader {
	         Tags 
         { 
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
	Pass {
		//Parametrage du shader pour éviter de lire, écrire dans le zbuffer, désactiver le culling et le brouillard sur le polygone

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			float4 _OutlineColor;
			float4 _SecondOutlineColor;
			sampler2D _MainTex;
			int isActive;

			struct Prog2Vertex {
	            float4 vertex : POSITION; 	//Les "registres" précisés après chaque variable servent
	            float4 tangent : TANGENT; 	//A savoir ce qu'on est censé attendre de la carte graphique.
	            float3 normal : NORMAL;		//(ce n'est pas basé sur le nom des variables).
	            float4 texcoord : TEXCOORD0;  
	            float4 texcoord1 : TEXCOORD1; 
	            fixed4 color : COLOR; 
        	 };
			 
			//Structure servant a transporter des données du vertex shader au pixel shader.
			//C'est au vertex shader de remplir a la main les informations de cette structure.
			struct Vertex2Pixel
			 {
           	 float4 pos : SV_POSITION;
           	 float4 uv : TEXCOORD0;

			 };  	 

			Vertex2Pixel vert (Prog2Vertex i)
			{
				Vertex2Pixel o;
		        o.pos = UnityObjectToClipPos (i.vertex); //Projection du modèle 3D, cette ligne est obligatoire
		        o.uv=i.texcoord; //UV de la texture
		      	
		      	return o;
			}

            float4 frag(Vertex2Pixel i) : COLOR 
            {
				 fixed4 c = tex2D(_MainTex, i.uv.xy);
				 c.rgb *= c.a;
				
				if(isActive == 1)
				{
					 float spriteLeft = step(0.9,tex2D(_MainTex, i.uv.xy - float2(0.02, 0)).a);
					 float spriteRight = step(0.9, tex2D(_MainTex, i.uv.xy + float2(0.02,  0)).a);
					 float spriteBottom = step(0.9, tex2D(_MainTex, i.uv.xy - float2( 0 ,0.01)).a);
					 float spriteTop = step(0.9, tex2D(_MainTex, i.uv.xy + float2( 0 , 0.02)).a);
				   
					 float result = (spriteRight + spriteLeft + spriteTop+ spriteBottom);
					 result *= (1-c.a);
					 float4 outlines = result * lerp(_OutlineColor, _SecondOutlineColor, cos(_Time.y * 2));				
					
					 c.rgb += outlines;
				}
				 return  c ;
            }
ENDCG 
	}
}

Fallback off

}