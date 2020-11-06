// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/SpriteToWhite" {
     Properties {
         _MainTex ("Font Texture", 2D) = "white" {}
         _Color ("Text Color", Color) = (1,1,1,1)
         
         _ColorMask ("Color Mask", Float) = 15
     }
 
     SubShader {
 
         Tags 
         {
             "Queue"="Transparent"
             "IgnoreProjector"="True"
             "RenderType"="Transparent"
         }
         
         
         
         Lighting Off 
         Cull Off 
         ZTest Off
         ZWrite Off 
         Blend SrcAlpha OneMinusSrcAlpha
         ColorMask [_ColorMask]
 
         Pass 
         {
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
 
             #include "UnityCG.cginc"
 
             struct appdata_t {
                 float4 vertex : POSITION;
                 fixed4 color : COLOR;
                 float2 texcoord : TEXCOORD0;
             };
 
             struct v2f {
                 float4 vertex : SV_POSITION;
                 fixed4 color : COLOR;
                 float2 texcoord : TEXCOORD0;
             };
 
             sampler2D _MainTex;
             uniform float4 _MainTex_ST;
             uniform fixed4 _Color;
             
             v2f vert (appdata_t v)
             {
                 v2f o;
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.color = v.color * _Color;
                 o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
 #ifdef UNITY_HALF_TEXEL_OFFSET
                 o.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
 #endif
                 return o;
             }
 
             fixed4 frag (v2f i) : SV_Target
             {
                 fixed4 col = i.color;
                 col.a *= tex2D(_MainTex, i.texcoord).a;
                 clip (col.a - 0.01);
                 return col;
             }
             ENDCG 
         }
     }
 }