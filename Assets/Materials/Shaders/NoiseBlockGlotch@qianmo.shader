Shader "Unlit/NoiseBlockGliatch@qianmo"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amplitude ("Amplitude", Range(-1, 0)) = -0.15
        _Amount ("Amount", Range(-5, 5)) = 0.5
        _BlockSize ("Block Size", Range(0, 1)) = 0.05
        _Speed ("Speed", Range(0, 100)) = 10
        _BlockPow ("Block Size Pow", Vector) = (3, 3, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _Amplitude;
            float _Amount;
            float _BlockSize;
            float _Speed;
            float4 _BlockPow;
            
            inline float randomNoise(float2 seed)
            {
                return frac(sin(dot(seed * floor(_Time.y * _Speed), float2(17.13, 3.71))) * 43758.5453123);
            }
            inline float randomNoise(float seed)
            {
                return randomNoise(float2(seed, 1.0));
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed2 screenUV = fixed2(i.uv.x * _ScreenParams.x, i.uv.y * _ScreenParams.y);
                half2 block = randomNoise(floor(screenUV * _BlockSize));
                float displaceNoise = pow(block.x, 8.0) * pow(block.x, 3.0);
                half R = tex2D(_MainTex, i.uv).r;
                half G = tex2D(_MainTex, i.uv + float2(displaceNoise * 0.05 * randomNoise(7.0), 0.0)).g;
                half B = tex2D(_MainTex, i.uv - float2(displaceNoise * 0.05 * randomNoise(13.0), 0.0)).b;
                return half4(R, G, B, 1.0);
            }
            ENDCG
        }
    }
}
