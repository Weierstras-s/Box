Shader "Custom/Clip" {
    Properties {
        _Color("Main Color", Color) = (1, 1, 1, 1)
        _MainTex("Base (RGB)", 2D) = "white" {}
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
            uniform fixed4 _Color;
            float4 _MainTex_ST;
            float3 _Direction;
            float _Height;

            struct v2f {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 light : TEXCOORD1;
                float4 vertex : TEXCOORD2;
            };

            v2f vert(appdata_full v) {
                v2f o;
                o.vertex = v.vertex;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.light = normalize(_WorldSpaceLightPos0.xyz);
                return o;
            }
            fixed4 frag(v2f i) : COLOR {
                clip(_Height - dot(i.vertex, normalize(_Direction)));
                float4 tex = tex2D(_MainTex, i.uv) * _Color;
                float3 ambientLight = UNITY_LIGHTMODEL_AMBIENT.rgb;
                fixed diff = max(0, dot(i.normal, i.light));
                float3 diffCol = _LightColor0.rgb * diff;
                return float4(ambientLight + diffCol, 1) * tex;
            }
            ENDCG
        }
    }
}
