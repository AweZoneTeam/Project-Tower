Shader "Example/Simple Additive"
{
	Properties
	{
		_MainTex("Texture to blend", 2D) = "white"{}
	}
	SubShader
	{
		Tags{"Queue" = "Transparent" }
		Pass
		{
			Blend SrcColor One //Alpha blending 
			//Blend One One //Additive
			//Blend OneMinusDstColor One //Soft Additive
			//Blend DstColor Zero //Multiplicative
			//Blend DstColor SrcColor //2x Multiplicative
			SetTexture[_MainTex] {combine texture}
		}	
	}
}