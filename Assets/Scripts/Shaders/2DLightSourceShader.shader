Shader "ProjectTower/2DLightSourceShader" 
{
	Properties
	{
		_MainTex("Main Texture",2D) = "white"{}
		_defaultZPosition("Default Z Position",float)=24.5
		_Light("light Intensity", Range(0,8)) = 0
		_LRadius("light Radius", float) = 10
		_LPos("light sourse position", Vector) = (.0, .0, .0, .0)
	}

		SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	float _defaultZPosition;
	uniform float _Light;
	uniform float _LRadius;
	uniform float4 _LPos;

	struct vertOut
	{
		float4 pos:SV_POSITION;
		float4 uv:TEXCOORD0;
	};

	vertOut vert(appdata_base v)
	{
		vertOut o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	fixed4 frag(vertOut i) : SV_Target
	{
		fixed4 color;
		float2 scrPos = float2(i.uv.x * _ScreenParams.x, i.uv.y * _ScreenParams.y);
		if (distance(scrPos, float2(_LPos.x * _ScreenParams.x,_LPos.y * _ScreenParams.y)) < _LRadius) 
		{
			color = fixed4(tex2D(_MainTex, i.uv).rgb*2, tex2D(_MainTex, i.uv).a);
		}
		else
		{
			color = fixed4(tex2D(_MainTex, i.uv));
		}
		return color;
	}
		ENDCG
	}
	}
}
