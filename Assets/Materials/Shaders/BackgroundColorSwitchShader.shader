Shader "Unlit/BackgroundColorSwitchShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Stage("Stage", Int) = 2
        _StartTime("StartTime", Float) = 0
        _OnceChangeTime("OnceChangeTime", Float) = 3.0
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
            #include "UnityCG.cginc"
            static const float4x4 _InMatrix = fixed4x4(
                    0.7647, 0.92549, 0.8941176, 1,
                    0.698039, 0.8, 0.8980, 1,
                    0.88627, 0.90196, 0.6039, 1,
                    0.9647, 0.58431, 0.58431, 1);
            static const float4x4 _OutMatrix = fixed4x4(
                    0.8588, 0.9647, 0.9372,1,
                    0.72941176, 0.83529, 0.92941176, 1,
                    0.917647, 0.92549, 0.7294117,1,
                    0.96078, 0.62745, 0.6392156,1);
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
            float4 _MainTex_ST;
            int _Stage;
            float _StartTime;
            float _OnceChangeTime;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float get_switch_col_linear(float percent, float y1, float y2)
            {
                float b = y1;
                float a = y2 - y1;
                return percent * a + b;
            }
            float get_switch_col_throw_climb(float percent, float y1, float y2)
            {
                float b = y1;
                float a = y2 - y1;
                return percent * percent * a + b;
            }
            float get_switch_col_sharp_return(float percent, float y1, float y2)
            {
                float b = 2 - y1;
                float a = y2 + y1 - 2;
                float res = a * percent + b;
                res = step(res, 1) * res + step(1, res) * (2 - res); 
                return res;
            }
            float4 get_switch_col(float percent, float4 y1, float4 y2)
            {
                float r = get_switch_col_linear(percent, y1.r, y2.r);
                float g = get_switch_col_throw_climb(percent, y1.g, y2.g);
                float b = get_switch_col_throw_climb(percent, y1.b, y2.b);
                return float4(r, g ,b, 1);
            }
            float4 get_switch_col_in(float percent, float4 y1, float4 y2)
            {
                float r = get_switch_col_sharp_return(percent, y1.r, y2.r);
                float g = get_switch_col_sharp_return(percent, y1.g, y2.g);
                float b = get_switch_col_sharp_return(percent, y1.b, y2.b);
                return float4(r, g ,b, 1);
            }
            
            fixed4 showCol(fixed f1)
            {
                return fixed4(f1, f1, f1, 0);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float sinceStartTime = _Time.y - _StartTime;
                float percent = sinceStartTime / _OnceChangeTime;
                
                fixed4 InCol = _InMatrix[_Stage];
                fixed4 OutCol = _OutMatrix[_Stage];
                if(sinceStartTime > 0 && sinceStartTime < _OnceChangeTime)
                {
                    //更新了startTime的时候，stage也会被同时更新
                    InCol = get_switch_col_in(percent, _InMatrix[_Stage - 1], _InMatrix[_Stage]);
                    OutCol = get_switch_col(percent, _OutMatrix[_Stage - 1], _OutMatrix[_Stage]);
                }
                //float percent = fmod(_Time.y, 3) / 3;
                
                fixed4 BaseCol = tex2D(_MainTex, i.uv);
                if(BaseCol.r < 0.4)
                {
                    return InCol;
                }
                return OutCol;
                
            }
            ENDCG
        }
    }
}
