Shader "Unlit/PixelCollapseShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _StartTime("StartTime", float) = 0
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "true"
            "RenderType"="Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        //ZTest LEqual
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
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _StartTime = 999999;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            fixed4 showCol(float a)
            {
                return fixed4(a, a, a, 1);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float uv_step = 0.05;
                float pixelized_time = 0.4;
                float fading_time = 0.4;
                float uv_step_start = 0.01;
                float uv_step_end = 0.08;
                fixed4 main_color = tex2D(_MainTex, i.uv);
                
                fixed4 transparent_color = fixed4(0, 0, 0, 0);
                
                if(_Time.y < _StartTime)
                {
                    return main_color;
                }
                fixed2 pixelized_uv;
                fixed4 pixelized_color;
                if(_Time.y - _StartTime < pixelized_time)
                {
                    uv_step = uv_step_start + (uv_step_end - uv_step_start) * (_Time.y - _StartTime) / pixelized_time;
                    pixelized_uv = floor(i.uv / uv_step) * uv_step;
                    pixelized_color = tex2D(_MainTex, pixelized_uv);
                    return pixelized_color;
                }
                if(_Time.y - _StartTime > pixelized_time && _Time.y - _StartTime < pixelized_time + fading_time)
                {
                    pixelized_uv = floor(i.uv / uv_step_end) * uv_step_end;
                    pixelized_color = tex2D(_MainTex, pixelized_uv);
                    float fading_percent = (_Time.y - _StartTime - pixelized_time) / fading_time;
                    float pixelNoInAll = (pixelized_uv.x + pixelized_uv.y) / 2;
                    if(pixelNoInAll < fading_percent)
                    {
                        pixelized_color.a = 0;
                    }
                    return pixelized_color;
                }
                //return fixed4(pixelized_uv.x, pixelized_uv.y, 0, 1);
                return transparent_color;
            }
            ENDCG
        }
    }
}
