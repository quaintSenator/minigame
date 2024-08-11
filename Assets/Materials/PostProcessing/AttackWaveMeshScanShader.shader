Shader "SF/AttackWaveMeshScanShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ScanTex("ScanTexure",2D) = "white"{}
		_ScanWidth("ScanWidth",float) = 0.2
    	_MaxR("MaxR", float) = 0.7
    	_TimeOfStart("TimeOfStart", float) = -999
    	_TimeOfRepeat("TimeOfRepeat", float) = 1.2
    }
    SubShader
    {
    	Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 100
        // No culling or depth
        ZWrite Off
	    ZTest LEqual
	    Blend SrcAlpha OneMinusSrcAlpha
	    
	    //Blend SrcAlpha OneMinusSrcAlpha
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _ScanTex;
			float _ScanWidth;
			float _MeshWidth;
            float _MaxR;
            float _TimeOfStart = -999;
    		float _TimeOfRepeat = 1.2;
            fixed4 showColor(float a)
            {
	            return fixed4(a, a, a, 1);
            }
            fixed4 frag (v2f i) : SV_Target
            {
				float scanRange = (_Time.y - _TimeOfStart) / _TimeOfRepeat;
            	//fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col = fixed4(1, 1, 1, 0);
				fixed4 scanMeshCol = tex2D(_ScanTex, i.uv);
                float pixelDistance = distance(i.uv, float2(0.5, 0.5));
				if(scanRange - pixelDistance > 0 && scanRange - pixelDistance <_ScanWidth && scanRange < _MaxR){
					fixed scanPercent = 1 - (scanRange - pixelDistance)/_ScanWidth;
					col = lerp(col, scanMeshCol, scanPercent);
				}
                return col;
                //return showColor(i.uv.x);
            }
            ENDCG
        }
    }
}
