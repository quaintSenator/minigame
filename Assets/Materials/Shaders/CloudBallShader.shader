Shader "Unlit/CloudBallShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            float RandFast( float2 PixelPos, float Magic = 3571.0 )
            {
            	float2 Random2 = ( 1.0 / 4320.0 ) * PixelPos + float2( 0.25, 0.0 );
            	float Random = frac( dot( Random2 * Random2, Magic ) );
            	Random = frac( Random * Random * (2 * Magic) );
            	return Random;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float r = distance(fixed2(0.5, 0.5), i.uv);
                float4 col = tex2D(_MainTex, i.uv);//原图越白，alpha越小
                col.a = (1 - col.r) * 2.9;
                col.g = col.g * 2.77;
                col.b = col.b * 2.77;
                col.r = col.r * 2.77;
                //col.a = RandFast(i.uv);
                //col.a = (1 / col.a) - 2.22;
                return col;
            }
            ENDCG
        }
    }
}
