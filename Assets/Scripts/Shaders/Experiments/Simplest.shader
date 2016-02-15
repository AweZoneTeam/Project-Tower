//colored vertex lighting
Shader "Example/Simple Colored Lighting"
{
	//a single color property
	Properties
	{
		_Color("Main Color", Color) = (1,.5,.5,1)
	}
	//define one subshader
	SubShader
	{
		//a single pass in our subshader
		Pass
		{
			//use fixed function per vertex-lighting
			Material
			{
				Diffuse[_Color]
			}
			Lighting On
		}
	}
}