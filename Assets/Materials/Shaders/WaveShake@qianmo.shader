Shader "Unlit/WaveShake"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Amount ("Amount", Range(-5, 5)) = 0.5
        _Speed ("Speed", Range(0, 100)) = 10
        _Frequency ("Frequency", Range(0, 100)) = 10
        _RGBSplit ("RGBSplit", Range(0, 1000)) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_horizontal

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
            float _Amount;
            float _Speed;
            float _Frequency;
            float NOISE_SIMPLEX_1_DIV_289 = 0.003460207612456747404844;
            float _RGBSplit;
            float2 mod289(float2 x)
            {
                return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
            }
            float3 mod289(float3 x)
            {
                return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
            }
            float3 permute(float3 x)
            {
                return mod289(x * x * 34.0 + x);
            }
            float3 taylorInvSqrt(float3 r)
            {
                return 1.79284291400159 - 0.85373472095314 * r;
            }
            float snoise(float2 v)
            {
                const float4 C = float4(0.211324865405187, // (3.0-sqrt(3.0))/6.0
                0.366025403784439, // 0.5*(sqrt(3.0)-1.0)
                - 0.577350269189626, // -1.0 + 2.0 * C.x
                0.024390243902439); // 1.0 / 41.0
                
                // First corner
                float2 i = floor(v + dot(v, C.yy));
                float2 x0 = v - i + dot(i, C.xx);
                
                // Other corners
                float2 i1;
                i1.x = step(x0.y, x0.x);
                i1.y = 1.0 - i1.x;
                
                // x1 = x0 - i1  + 1.0 * C.xx;
                // x2 = x0 - 1.0 + 2.0 * C.xx;
                float2 x1 = x0 + C.xx - i1;
                float2 x2 = x0 + C.zz;
                
                // Permutations
                i = mod289(i); // Avoid truncation effects in permutation
                float3 p = permute(permute(i.y + float3(0.0, i1.y, 1.0))
                + i.x + float3(0.0, i1.x, 1.0));
                
                float3 m = max(0.5 - float3(dot(x0, x0), dot(x1, x1), dot(x2, x2)), 0.0);
                m = m * m;
                m = m * m;
                
                // Gradients: 41 points uniformly over a line, mapped onto a diamond.
                // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
                float3 x = 2.0 * frac(p * C.www) - 1.0;
                float3 h = abs(x) - 0.5;
                float3 ox = floor(x + 0.5);
                float3 a0 = x - ox;
                
                // Normalise gradients implicitly by scaling m
                m *= taylorInvSqrt(a0 * a0 + h * h);
                
                // Compute final noise value at P
                float3 g;
                g.x = a0.x * x0.x + h.x * x0.y;
                g.y = a0.y * x1.x + h.y * x1.y;
                g.z = a0.z * x2.x + h.z * x2.y;
                return 130.0 * dot(m, g);
            }
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
            fixed4 showColor(float a)
            {
                return fixed4(a, a, a, 1);
            }
            fixed4 frag_horizontal (v2f i) : SV_Target
            {
                //fixed2 screenUV = fixed2(i.uv.x * _ScreenParams.x, i.uv.y * _ScreenParams.y);
                fixed2 screenUV = i.uv;
                float uvy = i.uv.y * _ScreenParams.y;
                float noise_wave_1 = snoise(float2(uvy * 0.01, _Time.y * _Speed * 10)) * (_Amount * 32.0);
                float noise_wave_2 = snoise(float2(uvy * 0.02, _Time.y * _Speed * 5)) * (_Amount * 4.0);
                float noise_wave_x = noise_wave_1 * noise_wave_2 / _ScreenParams.x;

                float uvx = i.uv.x + noise_wave_x;
                //screenUV.x = (screenUV.x + noise_wave_x) /_ScreenParams.x;
                
                //float rgbSplit_uv_x = (RGBSplit * 50 + (20.0 + 1.0)) * noise_wave_x;
                float rgbSplit_uv_x = (_RGBSplit * 50 + 21.0) * noise_wave_x / _ScreenParams.x;
                half4 normalColor = tex2D(_MainTex, fixed2(uvx, i.uv.y));
                half4 errorColor = tex2D(_MainTex, fixed2(uvx + rgbSplit_uv_x, i.uv.y));
                
                //return half4(normalColor.r, normalColor.g, errorColor.b, 1.0);
                return half4(normalColor.r, normalColor.g, errorColor.b, 1.0);
                //return showColor(noise_wave_x * 1920);
            }
            ENDCG
        }
    }
}
