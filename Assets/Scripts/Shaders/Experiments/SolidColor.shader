Shader "Example/SolidColor" {
	Properties
	{	
		_Color("Main Color", Color) = (0,0,0,0)
		_MainTex("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Blend SrcAlpha One
		ZWrite Off
		Pass
		{
			Color[_Color]
			SetTexture[_MainTex] {Combine primary*texture}
			SetTexture[_MainTex]{Combine previous alpha }
		}
	}
}
