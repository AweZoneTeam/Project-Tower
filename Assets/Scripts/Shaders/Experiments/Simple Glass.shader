Shader "Example/Simple Glass"
{
	Properties
	{
		_Color ("Main Color", Color)=(1,1,1,0)
		_SpecColor("Spec Color", Color)= (1,1,1,1)
		_EmissionColor("Emisive Color", Color)=(0,0,0,0)
		_Shininess("Shininess", Range(0.01,1))=0.7	
		_MainTex("Base (RGB)",2D) = "white"{}
	}
	SubShader
	{
		//We use the material in many passes by defining them in subshader
		//Anything defined here becomes default values for all contained passes.
		Material
		{
			Diffuse[_Color]
			Ambient[_Color]
			Shininess[_Shininess]
			Specular[_SpecColor]
			Emission [_Emission]
		}
		Lighting On
		SeparateSpecular On
		//Set up alpha blending
		Blend SrcAlpha OneMinusSrcAlpha

		//Render the back facing parts of the object
		//if the object is convex, this will always be the further way
		//than the front faces.
		Pass
		{
			Cull Back
			SetTexture [_MainTex]
			{
				Combine Primary*Texture
			}
		}
	}
}