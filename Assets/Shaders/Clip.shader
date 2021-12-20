// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Clip" {
    Properties {
        _Color("Main Color", Color) = (1, 1, 1, 1)
        _MainTex("Base (RGB)", 2D) = "white" {}
        _Alpha("Alpha", Float) = 0.0
        _DecalTex("Decal (RGBA)", 2D) = "black" {}
        _Height("Height", Float) = 0.5
        _Direction("Direction", Vector) = (1, 0, 0)
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        LOD 250
        Pass {
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"  
            #include "Lighting.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _DecalTex;
            uniform fixed4 _Color;
            float4 _MainTex_ST;
            float _Alpha;
            float4 _DecalTex_ST;
            float3 _Direction;
            float _Height;

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv_main : TEXCOORD0;
                float2 uv_decal : TEXCOORD1;
                float3 light : TEXCOORD2;
                float4 vertex : TEXCOORD3;
            };

            v2f vert(appdata_full v) {
                v2f o;
                o.vertex = v.vertex;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv_main = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv_decal = TRANSFORM_TEX(v.texcoord, _DecalTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.light = normalize(_WorldSpaceLightPos0.xyz);
                return o;
            }
            fixed4 frag(v2f i) : COLOR {
                clip(_Height - dot(i.vertex, normalize(_Direction)));
                float4 tex = tex2D(_MainTex, i.uv_main);
                float4 decal = tex2D(_DecalTex, i.uv_decal);
                tex.rgb = lerp(tex.rgb, decal.rgb, decal.a * _Alpha);
                tex *= _Color;
                float3 ambientLight = UNITY_LIGHTMODEL_AMBIENT.rgb;
                fixed diff = max(0, dot(i.normal, i.light));
                float3 diffCol = _LightColor0.rgb * diff;
                float4 col = float4(ambientLight + diffCol, 1) * tex;
                return col;
            }
            ENDCG
        }
    }
}
