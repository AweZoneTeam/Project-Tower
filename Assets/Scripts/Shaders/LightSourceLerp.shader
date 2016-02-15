Shader "Project Tower/LightSourceLerp"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
		_Scale("Scale", Float) = 1.0
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent+100" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

		ZWrite Off
		Blend SrcAlpha One

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog

#include "UnityCG.cginc"

	struct appdata_t
	{
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		half2 texcoord : TEXCOORD0;
		UNITY_FOG_COORDS(1)
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	fixed4 _Color;
	float _Scale;

	v2f vert(appdata_t v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float2 cPos = float2(0.5,0.5);
		float2 fPos = float2(1.0, 1.0);
		fixed4 col = tex2D(_MainTex, i.texcoord)*_Color*_Scale*(1.0-(distance(cPos,i.texcoord.xy)/distance(cPos,fPos)));
		UNITY_APPLY_FOG(i.fogCoord, col);
		return col;
	}
		ENDCG
	}
	}
}
