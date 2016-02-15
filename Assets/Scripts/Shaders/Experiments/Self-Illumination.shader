Shader "Example/Self-Illumination"
{
	Properties
	{
		_MainTex("Base (RGB) Self-Illumination (A)",2D) = "white"{}
	}
	SubShader
	{
		Pass
		{
			//Set up base verex lighting
			Material
			{
				Diffuse (1,1,1,1)
				Ambient(1,1,1,1)
			}
			Lighting On
			
			//Use texture alpha to blen up to white(= full illumination)
			SetTexture[_MainTex]
			{
				constantColor(1,1,1,1)
				combine constant lerp(texture) previous
			}
			//Multiply in texture
			SetTexture[_MainTex]
			{
				combine previous*texture
			}			
		}
	}
}