Shader "Example/Reveal Backfaces"
{
	Properties
	{
		_MainTex("Base (RGB)",2D) = "white"{}
	}
	Subshader
	{
		//renderthe front-facing parts of the object.
		//we use a simple white material, and apply themain texture.
		Pass
		{
			Material
			{
				Diffuse(1,1,1,1)
			}
			Lighting On
			SetTexture[_MainTex]
			{
				Combine Primary*Texture
			}
		}
		//Now we render the back-facin trianglesin the most
		//irritating color in the world: BRIGHT PINK!
		Pass {
			Color (1,0,1,1)
			Cull Front
		}
	}
}