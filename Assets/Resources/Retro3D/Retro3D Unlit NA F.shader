Shader "Retro3D/No Affine Transparent Fade"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
		_Illum("Illumination", 2D) = "black" {}
		_Color("Color", Color) = (0.5, 0.5, 0.5, 1)
		_GeoRes("Geometric Resolution", Float) = 40
		_Distance2("Clipping Start Distance", Float) = 10
		_Distance("Clipping End Distance", Float) = 60
    }
		Category{
			Cull Off
			SubShader
			{
				Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
				Blend SrcAlpha OneMinusSrcAlpha
				Pass
				{
					CGPROGRAM
					#include "UnityCG.cginc"

					#pragma vertex vert
					#pragma fragment frag

					struct v2f
					{
						float4 position : SV_POSITION;
						half2 distance : TEXCOORD1;
						half2 texcoord : TEXCOORD;
						fixed4 color : COLOR;
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;
					sampler2D _Illum;
					float4 _Color;
					float _GeoRes;
					float _Distance;
					float _Distance2;
					fixed _Cutoff;

					v2f vert(appdata_full v)
					{
						v2f o;
						float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
						wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

						float4 sp = mul(UNITY_MATRIX_P, wp);
						o.position = sp;
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						fixed4 newC = v.color;
						o.color = newC * _Color;
						o.distance = wp.xy;
						o.distance.x = distance(floor(_WorldSpaceCameraPos * 0.3) / 0.3, mul(_Object2World, v.vertex));
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 col = (tex2D(_MainTex, i.texcoord) * i.color) + tex2D(_Illum, i.texcoord);
						col.a = tex2D(_MainTex, i.texcoord).a;
						col.a *= 1 - ((-_Distance2 + i.distance.x)/(-_Distance2 + _Distance));
						clip(i.distance.x < _Distance ? 1 : -1);
						return col;
					}

					ENDCG
				}
			}
		}
}
