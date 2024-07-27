Shader "SF/ExpandingCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "blue" {}
        _CircleMaxRadium("CircleMaxRadium", float) = 0.48
        _OuterCircleWidth("OuterCircleWidth", float) = 0.02
        _OuterCircleColor("OuterCircleColor", color) = (0, 0, 0, 1)
        _GrowingCircleColor("GrowingCircleColor", color) = (1, 0.3, 0.2, 1)

        ///Start added by yeniao 
        _NormalColor("NormalColor", color) = (0.0, 0.0, 0.0,1.0)
        ///End
        _PerfectColor("PerfectColor", color) = (0.3, 0.85, 0.2, 1)
        
         ///Start added by yeniao 
         _NormalRangeStartTime("NormalRangeStartTime", float) = 0.4
         _NormalRangeEndTime("NormalRangeEndTime", float) = 0.8
         ///End
        
        _BeatStartTime("BeatStartTime", float) = 0
        _PerfectRangeStartTime("PerfectRangeStartTime", float) = 0.5
        _PerfectRangeEndTime("PerfectRangeEndTime", float) = 0.7
        _BeatEndTime("BeatEndTime", float) = 1.2
        _IntervalTimeB2WBeats("IntervalTimeB2WBeats", float) = 0
    }
    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100

        Pass
        {
            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #include "UnityCG.cginc"
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
            float _CircleMaxRadium;
            float _OuterCircleWidth;
            fixed4 _OuterCircleColor;
            fixed4 _GrowingCircleColor;

            fixed4 _NormalColor;

            fixed4 _PerfectColor;
            
            float _BeatStartTime;
            float _PerfectRangeStartTime;
            float _PerfectRangeEndTime;

            float _NormalRangeStartTime;
            float _NormalRangeEndTime;

            float _BeatEndTime;
            float _IntervalTimeB2WBeats;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            float isInRange(float a, float b, float r)
            {
                return step(a, r) * step(r, b);
            }
            
            fixed4 showColor(float a)
            {
                return fixed4(a, a, a, 1);
            }
            fixed4 frag (v2f i) : SV_Target
            {

                //fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 transparentColor = fixed4(1, 1, 1, 0);
                float r = pow(i.uv.x - 0.5, 2.0) + pow(i.uv.y - 0.5, 2.0);
                r = pow(r, 0.5);
                float isAtCorner = step(_CircleMaxRadium, r);
                float outerR = _OuterCircleWidth + _CircleMaxRadium;
                float isAtOuterCircle = isInRange(_CircleMaxRadium, outerR, r);
                
                float totalPeriod = _BeatEndTime + _IntervalTimeB2WBeats;
                float timeInPeriod = fmod(_Time.y, totalPeriod);      
              
                float clamptTimeInPeriodToNormalRangeStartTime=clamp(timeInPeriod - _BeatStartTime,0,_NormalRangeStartTime);
                //Modify by yeniao
                //Source:float innerCircleR = _CircleMaxRadium * (timeInPeriod - _BeatStartTime) / (_BeatEndTime - _BeatStartTime);
                float innerCircleR=_CircleMaxRadium *  clamptTimeInPeriodToNormalRangeStartTime/(_NormalRangeStartTime - _BeatStartTime);

                float ifNormal=isInRange(_NormalRangeStartTime, _NormalRangeEndTime, timeInPeriod);

                float isPerfect = isInRange(_PerfectRangeStartTime, _PerfectRangeEndTime, timeInPeriod);
                
                
                float isAtOutsideGrowingCircle = isInRange(innerCircleR, _CircleMaxRadium, r);
                float isAtGrowingCircle = step(r, innerCircleR);

                fixed4 growingColor = lerp(_NormalColor, _GrowingCircleColor, isPerfect);
                fixed4 normalLerpToPerfectColor=lerp(_NormalColor,_PerfectColor,ifNormal);
                
                fixed4 col = _OuterCircleColor * isAtOuterCircle +
                    transparentColor * (isAtCorner + isAtOutsideGrowingCircle) + 
                    isAtGrowingCircle * growingColor +
                    normalLerpToPerfectColor;
                return col;
            }
            ENDCG
        }
    }
}
