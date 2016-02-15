Shader "Example/Diffuse Texture"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white"{}
		_Alpha ("Alpha", Range (0,1))=1
	}
	SubShader
	{
		Tags {"RenderType"="Opaque"}
		CGPROGRAM
		#pragma surface surf Lambert
		struct Input
		{
			float2 uv_MainTex;
		};
		sampler2D _MainTex;
		uniform float _Alpha;

		void surf(Input IN, inout SurfaceOutput o)
		{
			o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
			o.Alpha *= _Alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}