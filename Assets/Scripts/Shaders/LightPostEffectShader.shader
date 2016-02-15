Shader "Project Tower/LightPostEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LightTex("Light Map Texture",2D) = "white"{}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			#include "UnityCG.cginc"
	
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
	
			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 l_uv : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};
	
			uniform sampler2D _MainTex;
			uniform sampler2D _LightTex;
			uniform float4 _MainTex_TexelSize;
	
				
			v2f vert(appdata v)
			{
				v2f o;	
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.l_uv = o.uv;
					
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
				{
					o.l_uv.y = 1 - o.l_uv.y;
				}
				#endif

				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex,i.uv);
				fixed4 lightCol = tex2D(_LightTex, i.l_uv);
				col = col*lightCol;
				return col;
			}

			ENDCG
		}
	}
}
