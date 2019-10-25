Shader "Custom/SpiderMaterialShader"
{
	Properties
	{
	_MainTex("Albedo (RGB)", 2D) = "white" {}
	_Color("Color",color) = (1,1,1,1)
	_Cutoff("Alpha Cut", Range(0, 1)) = 0
	_Thick("Thick Cut", Range(0, 1)) = 0
	[HDR]_OutColor("OutColor", Color) = (1, 1, 1, 1)
	_OutThinkness("OutThinkness", Range(1, 1.5)) = 1.15
	_EmissionColor("EmissionColor", Color) = (0, 0, 0, 0)
	}
		SubShader
	{
	Tags { "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" }
	CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;

		float4 _OutColor;
		float _OutThinkness;
		float _Thick;
		float4 _EmissionColor;


		struct Input
		{
		float2 uv_MainTex;
		float2 uv_NoiseTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Emission = _EmissionColor + step(c.a, _Thick * _OutThinkness) * _OutColor.rgb;
			}
			ENDCG
	}
		Fallback "Legacy Shaders/Transparent/Cutout/Diffuse"
}