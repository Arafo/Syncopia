Shader "BallisticNG/SkyCar"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
		_Illum("Illumination", 2D) = "black" {}
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
		_CarColor("Car Color", Color) = (0.5, 0.5, 0.5, 1)
        _GeoRes("Geometric Resolution", Float) = 40
		_Distance("Clipping Distance", Float) = 60
    }
		Category{
			Cull Off
			SubShader
			{
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
					};

					sampler2D _MainTex;
					sampler2D _Illum;
					float4 _MainTex_ST;
					float4 _Color;
					float4 _CarColor;
					float _GeoRes;
					float _Distance;

					v2f vert(appdata_full v)
					{
						v2f o;
						float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
						wp.xyz = floor(wp.xyz * _GeoRes) / _GeoRes;

						float4 sp = mul(UNITY_MATRIX_P, wp);
						o.position = sp;
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						o.distance = wp.xy;
						o.distance.x = distance(floor(_WorldSpaceCameraPos * 0.3) / 0.3, mul(_Object2World, v.vertex));
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						clip(i.distance.x < _Distance ? 1 : -1);
						float4 base = tex2D(_MainTex, i.texcoord) * _Color;
						float4 final = lerp(base, base * _CarColor, tex2D(_MainTex, i.texcoord).a);
						return lerp(final, tex2D(_Illum, i.texcoord), tex2D(_Illum, i.texcoord));
					}

					ENDCG
				}
			}
		}
}
