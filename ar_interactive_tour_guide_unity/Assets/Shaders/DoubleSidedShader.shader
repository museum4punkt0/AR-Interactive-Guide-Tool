// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Standard-DoubleSided_solidback"
{
	 Properties {
         _Color ("Main Color", Color) = (1,1,1,1)
         _backFaceColor("backfaceColor", Color) = (0.5,0.3,0.15,1) // std brown backfacecolor
         _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
         _EmissionIntensity ("Emission intensity", Range(0.0,20.0)) = 0   
		 _EmissionMap("Emission", 2D) = "white" {}
     }
     
     SubShader {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
         LOD 200

         // extra pass that renders to depth buffer only to avoid the transparent render issues
         Pass {
            ZWrite On
            ColorMask 0
         }

              //  render back faces first
         Cull Front
         CGPROGRAM
         #pragma surface surf Lambert alpha
          
         fixed4 _backFaceColor;
         fixed4 _Color;

         struct Input {
             float2 uv_MainTex;
         };
 
         // solid color backfaces with same alpha as frontfaces
         void surf (Input IN, inout SurfaceOutput o) {
             o.Albedo = _backFaceColor.rgb; 
             o.Alpha = _Color.a;
         }
         ENDCG


         //now render front faces with the highlight texture on the emission chanel
         Cull Back
         CGPROGRAM
         #pragma surface surf Lambert alpha
         
         sampler2D _MainTex;
         sampler2D _EmissionMap;
         fixed4 _Color;
         fixed4 _backFaceColor;
         float _EmissionIntensity;
         float max;

         struct Input {
             float2 uv_MainTex;
             float2 uv_Illum;
         };
         
         void surf (Input IN, inout SurfaceOutput o) {
             fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
             o.Albedo = c.rgb;
             float em =  tex2D(_EmissionMap, IN.uv_MainTex).g  *_EmissionIntensity * c;
             max = 1.2;
             if(em>max) em = max;   // limit the maxEmission
             o.Emission = em;
             o.Alpha = c.a;
         }
         ENDCG

     
     }
     
     Fallback "Transparent/VertexLit"
}
