Shader "Unlit/ControllableExpandingCircle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "blue" {}
        _CircleMaxRadium("CircleMaxRadium", float) = 0.48
        _OuterCircleWidth("OuterCircleWidth", float) = 0.02
        _OuterCircleColor("OuterCircleColor", color) = (0, 0, 0, 1)
        _GrowingCircleColor("GrowingCircleColor", color) = (1, 0.3, 0.2, 1)
        _PerfectColor("PerfectColor", color) = (0.3, 0.85, 0.2, 1)
        
        _CallTime("CallTime", float) = 0
        _HoldTime("HoldTime", float) = 1.2
        _BeatStartTime("BeatStartTime", float) = 1.4
        _PerfectRangeStartTime("PerfectRangeStartTime", float) = 1.6
        _PerfectRangeEndTime("PerfectRangeEndTime", float) = 1.8
        _BeatEndTime("BeatEndTime", float) = 2
        _IntervalTimeB2WBeats("IntervalTimeB2WBeats", float) = 0.2
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
            fixed4 _PerfectColor;

            float _CallTime;
            float _HoldTime;
            float _BeatStartTime;
            float _PerfectRangeStartTime;
            float _PerfectRangeEndTime;
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
                fixed4 transparentColor = fixed4(1, 1, 1, 0);
                float r = pow(i.uv.x - 0.5, 2.0) + pow(i.uv.y - 0.5, 2.0);
                r = pow(r, 0.5);
                float isAtCorner = step(_CircleMaxRadium, r);
                float outerR = _OuterCircleWidth + _CircleMaxRadium;
                float isAtOuterCircle = isInRange(_CircleMaxRadium, outerR, r);
                
                float totalPeriod = _BeatEndTime + _IntervalTimeB2WBeats;
                float timeInPeriod = fmod(_Time.y, totalPeriod);
                float isInsideBeat = isInRange(_BeatStartTime, _BeatEndTime, timeInPeriod);
                float innerCircleR = _CircleMaxRadium * (timeInPeriod - _BeatStartTime) / (_BeatEndTime - _BeatStartTime);

                //如果还在HoldTime之前，直接毁坏innerCircleR设为0
                innerCircleR = step(_HoldTime, _Time.y) * _CircleMaxRadium * (_Time.y - _HoldTime) / (_BeatStartTime - _HoldTime);
                
                //如果已经超过fulltime，直接毁坏innerCircleR设为1
                float isAfterFullTime = step(_BeatStartTime, _Time.y);
                innerCircleR = lerp(innerCircleR, 1, isAfterFullTime);
                
                float isPerfect = isInRange(_PerfectRangeStartTime, _PerfectRangeEndTime, _Time.y);
                
                float isAtOutsideGrowingCircle = isInRange(innerCircleR, _CircleMaxRadium, r);
                float isAtGrowingCircle = step(r, innerCircleR);
                fixed4 growingColor = lerp(_PerfectColor, _GrowingCircleColor, isPerfect);

                
                fixed4 notDistroyedCol = isAtOutsideGrowingCircle * transparentColor + _OuterCircleColor * isAtOuterCircle + isAtGrowingCircle * growingColor;
                float isDistroyed = step(_Time.y, _BeatEndTime);
                fixed4 DistroyedCol = lerp(fixed4(0, 0, 0, 1), notDistroyedCol, isDistroyed);
                fixed4 resultCol = lerp(DistroyedCol, transparentColor, isAtCorner);//如果位于四角，破坏为transparentColor
                
                return resultCol;
                //isAtOuterCircle✓, isAtCorner✓, isAtOutsideGrowingCircle, isAtGrowingCircle
            }
            ENDCG
        }
    }
}
