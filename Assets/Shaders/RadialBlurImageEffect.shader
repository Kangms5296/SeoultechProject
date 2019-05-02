Shader "ImageEffect/RadialBlurImageEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
			ZTest Always Cull off Zwrite off
			Fog {Mode off}

            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
			#pragma target 3.0
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
			half4 _MainTex_TexelSize;
			half2 _BlurCenterPos;
			half _BlurSize;
			half _Samples;

            fixed4 frag (v2f_img IN) : SV_Target
            {
				half4 col = half4(0,0,0,1);
				half2 movedTexcoord = IN.uv - _BlurCenterPos;

				for (int i = 0; i < _Samples; i++)
				{
					half Scale = 1.0f - _BlurSize * _MainTex_TexelSize * i;
					col.rgb += tex2D(_MainTex, movedTexcoord * Scale + _BlurCenterPos).rgb;
				}

                col.rgb  *= 1/ _Samples;
                return col;
            }
            ENDCG
        }
    }
}
