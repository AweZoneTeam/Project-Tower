Shader "ProjectTower/2DDarkenShader" 
{	
	Properties
	{
	_MainTex("Main Texture",2D) = "white"{}
	_Darkness("darkness", Range(0,1)) = 0
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
			uniform float _Darkness;

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
				color = fixed4 (tex2D(_MainTex, i.uv).rgb, tex2D(_MainTex, i.uv).a*(1 - _Darkness));
				return color;
			}
			ENDCG
		}
	}
}
