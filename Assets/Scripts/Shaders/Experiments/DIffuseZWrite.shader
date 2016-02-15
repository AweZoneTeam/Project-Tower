Shader "Example/DIffuse ZWrite"
{
	Properties
	{
		_Color("Main Color",Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white"{}
		_Alpha("Alpha", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags{ Queue = Transparent }

		BindChannels
	{
		Bind "vertex", vertex
		Bind "color", color
		Bind "texcoord", texcoord
	}
	CGPROGRAM
	#pragma surface surf Lambert
	struct Input 
	{
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
	float _Alpha;

	void surf(Input IN, inout SurfaceOutput o)
	{
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
	}
	ENDCG
	/*Pass
	{
	}
	*/
	}
	Fallback "Diffuse"
}