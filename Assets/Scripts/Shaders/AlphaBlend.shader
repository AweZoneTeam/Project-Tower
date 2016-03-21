Shader "ProjectTower/AlphaBlending"
{

Properties
{
	_MainTex ("Texture", 2D)=""
}

SubShader
{
	Blend SrcAlpha OneMinusSrcAlpha
	//Cull Front
	Tags {Queue=Transparent}
	
	BindChannels
	{
		Bind "vertex", vertex
		Bind "color", color
		Bind "texcoord1", texcoord
	}

	Pass
	{
		SetTexture[_MainTex]//{Combine texture alpha}
	}

}


}