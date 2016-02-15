Shader "VividHelix/PortalShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	_Radius("Radius", Range(10,200)) = 50
		_FallOffRadius("FallOffRadius", Range(0,40)) = 20
		_RelativePortals("RelativePortals", Vector) = (.25,.25,.75,.75)
	}
		SubShader{
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		uniform sampler2D _MainTex;
	uniform half _Radius;
	uniform half _FallOffRadius;
	uniform half4 _RelativePortals;

	struct vertOut {
		float4 pos:SV_POSITION;
		float4 uv:TEXCOORD0;
	};

	vertOut vert(appdata_base v) {
		vertOut o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	fixed4 frag(vertOut i) : SV_Target{
		float2 scrPos = float2(i.uv.x * _ScreenParams.x, i.uv.y * _ScreenParams.y);
		float lerpFactor = 0;
		float2 leftPos = float2(_RelativePortals.x * _ScreenParams.x,_RelativePortals.y * _ScreenParams.y);
		float2 rightPos = float2(_RelativePortals.z * _ScreenParams.x,_RelativePortals.w * _ScreenParams.y);
		if (distance(scrPos, leftPos) < _Radius) {
			lerpFactor = 1 - saturate((distance(scrPos, leftPos) - (_Radius - _FallOffRadius)) / _FallOffRadius);
			scrPos.x = scrPos.x + rightPos.x - leftPos.x;
			scrPos.y = scrPos.y + rightPos.y - leftPos.y;
		}
		else if (distance(scrPos, rightPos) < _Radius) {
			lerpFactor = 1 - saturate((distance(scrPos, rightPos) - (_Radius - _FallOffRadius)) / _FallOffRadius);
			scrPos.x = scrPos.x + leftPos.x - rightPos.x;
			scrPos.y = scrPos.y + leftPos.y - rightPos.y;
		}
		return lerp(tex2D(_MainTex, i.uv), tex2D(_MainTex, float2(scrPos.x / _ScreenParams.x, scrPos.y / _ScreenParams.y)), lerpFactor);
	}
		ENDCG
	}
	}
}