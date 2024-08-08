Shader "Unlit/ErrorColorSplitShader@qianmo"
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
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float timex = _Time.x * _Speed;
                float splitAmout = (1.0 + sin(timex * 6.0)) * 0.5;
                splitAmout *= 1.0 + sin(timex * 16.0) * 0.5;
                splitAmout *= 1.0 + sin(timex * 19.0) * 0.5;
                splitAmout *= 1.0 + sin(timex * 27.0) * 0.5;
                splitAmout = pow(splitAmout, _Amplitude);
                splitAmout *= (0.05 * _Amount);

                half3 finalColor;
                finalColor.r = tex2D(_MainTex, fixed2(i.uv.x + splitAmout, i.uv.y)).r;
                finalColor.g = tex2D(_MainTex, i.uv).g;
                finalColor.b = tex2D(_MainTex, fixed2(i.uv.x - splitAmout, i.uv.y)).b;
                
                return half4(finalColor, 1.0);
            }
            ENDCG
        }
    }
}
