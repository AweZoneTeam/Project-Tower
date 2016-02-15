Shader "Example/TextureCoordinates/Base" {
	SubShader{
		Pass{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0

#include "UnityCG.cginc"

	struct vertexInput {
		float4 vertex : POSITION;
		float4 texcoord0 : TEXCOORD0;
	};

	struct fragmentInput {
		float4 position : SV_POSITION;
		float4 texcoord0 : TEXCOORD0;
	};

	fragmentInput vert(vertexInput i) {
		fragmentInput o;
		o.position = mul(UNITY_MATRIX_MVP, i.vertex);
		o.texcoord0 = i.texcoord0;
		return o;
	}
	fixed4 frag(fragmentInput i) : SV_Target{
		return fixed4(i.texcoord0.xy,0.0,1.0);
	}
		ENDCG
	}
	}
}