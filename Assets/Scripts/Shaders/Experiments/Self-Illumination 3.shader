Shader "Example/Self-Illumination 3"
{
	Properties
	{
		_IlluminCol("Self-Illumination color (RGB)", Color) = (1,1,1,1)
		_Color("Main Color",Color) = (1,1,1,0)
		_SpecColor("Spec Color",Color) = (1,1,1,1)
		_Emission("Emissive Color",Color) = (0,0,0,0)
		_Shininess("Shininess",Range(0.01,1))=0.7
		_MainTex("Base (RGB) Self-Illumination (A)",2D) = "white"{}
	}
		SubShader
	{
		Pass
	{
		//Set up basic vertex lighting
		Material
	{
		Diffuse[_Color]
		Ambient[_Color]
		Shininess[_Shininess]
		Specular[_SpecColor]
		Emission[_Emission]
	}
		Lighting On

		//Use texture alpha to blend up to white(= full illumination)
		SetTexture[_MainTex]
	{
		//pull the color	 property in this blender
		constantColor[_IlluminCol]
		//and use the texture's alpha to blend between it and vertex color
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
