Shader "Unlit/RotatingStarFallShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BallTex ("_BallTex", 2D) = "white" {}
        _StartTime("StartTime", float) = -99999
        _OnceTime("OnceTime", float) = 0.45
        _Angle2Rotate("Angle2Rotate", float) = 30
        _FadeTime("FadeTime", float) = 0.5
    }
    SubShader
    {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "true"
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off
        //ZTest LEqual
        LOD 100

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _BallTex;
            float4 _MainTex_ST;
            float _StartTime;
            float _OnceTime;
            float _Angle2Rotate;
            float _FadeTime;
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
            fixed2 rotateUV(fixed2 uv, float anglePi)
            {
                fixed2x2 rotationMatrix = fixed2x2(
                    cos(anglePi), -sin(anglePi),
                    sin(anglePi), cos(anglePi)
                    );
                fixed2 res = mul(rotationMatrix, uv - 0.5) + 0.5;
                return res;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float angleToRotate = _Angle2Rotate * 3.14159 / 180;
                float2 uv = rotateUV(i.uv, angleToRotate);
                float u = uv.x;
                float v = uv.y;
                fixed4 transparentColor = (0, 0, 0, 0);
                if(_StartTime < 0 || _Time.y > _StartTime + _OnceTime + _FadeTime)
                {
                   return transparentColor;
                }
                float currentAngle = (_Time.y - _StartTime) * 3.1415926 / _OnceTime; //0 -> pi
                
                float R = 0.4;
                float costheta = dot(normalize(uv - fixed2(0.5, 0.5)), fixed2(1, 0)); //1 -> -1
                float currentTheta = acos(costheta); // 0 ~ pi
                float d = distance(uv, float2(0.5, 0.5));
                float nearest = abs(d - R);
                float r = 0.1 * R * (-costheta + 1);  
                
                float alpha = 2.2 * pow(currentTheta / 3.14159, 3)  / (currentAngle / 3.14159);
                //float tailSamplePercentX = (d - nearest) / (2 * r);
                float tailSamplePercentX = (d - (R - r)) / (2 * r);
                float tailSamplePercentY = ((currentTheta - 0.52) / 3.14159);
                fixed4 tailColor = tex2D(_BallTex, float2(tailSamplePercentX, tailSamplePercentY));
                tailColor.a = alpha;
                float alphaScale = 1;
                if(_Time.y > _StartTime + _OnceTime && _Time.y < _StartTime + _OnceTime + _FadeTime)
                {
                    currentAngle = 3.14159;
                    alphaScale = 1 - (_Time.y - _StartTime - _OnceTime) / _FadeTime;
                }
                
                fixed2 ballHeartVec = fixed2(cos(currentAngle), sin(currentAngle)) * R;
                fixed2 ballHeart = fixed2(0.5, 0.5) + ballHeartVec;
                float d2heart = distance(uv, ballHeart);
                fixed2 balluv = (uv - ballHeart) * (0.5/ r) + fixed2(0.5, 0.5);
                fixed4 ballColor = tex2D(_BallTex, balluv);
                
                tailColor.a = tailColor.a * alphaScale; 
                ballColor.a = tailColor.a;
                
                if(distance(uv, ballHeart) < r)
                {
                    return ballColor;
                }
                if(nearest < r && currentTheta < currentAngle && v > 0.5)
                {
                    return tailColor;
                }
                return transparentColor;
            }
            ENDCG
        }
    }
}
