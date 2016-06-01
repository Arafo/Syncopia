Shader "Retro3D/No Affine Alpha Cull"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
		_Illum("Illumination", 2D) = "black" {}
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _GeoRes("Geometric Resolution", Float) = 40
		_Distance("Clipping Distance", Float) = 60
    }
		Category{
			SubShader
			{
				Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
				Pass
				{
					CGPROGRAM
					#include "UnityCG.cginc"

					#pragma vertex vert
					#pragma fragment frag

					struct v2f
					{
						float4 position : SV_POSITION;
						half2 texcoord : TEXCOORD;
						half2 distance : TEXCOORD1;
						fixed4 color : COLOR;
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;
					sampler2D _Illum;
					float4 _Color;
					float _GeoRes;
					float _Distance;
					float3 pos;
					fixed _Cutoff;

					v2f vert(appdata_full v)
					{
						v2f o;

						pos = _WorldSpaceCameraPos;
						pos.y = mul(_Object2World, v.vertex).y;
						float dist = distance(floor(pos * 0.3) / 0.3, mul(_Object2World, v.vertex));
						float res = _GeoRes - (clamp(dist, 0, _GeoRes * 0.6));
						float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
						wp.xyz = floor(wp.xyz * res) / res;

						float4 sp = mul(UNITY_MATRIX_P, wp);
						o.position = sp;
						o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
						fixed4 newC = v.color;
						newC.a = 1.0;
						o.color = newC * _Color;

						o.distance = wp.xy;
						o.distance.x = dist;
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 col = (tex2D(_MainTex, i.texcoord) * i.color);
						i.distance.y = col.a;
						col += tex2D(_Illum, i.texcoord);
						col.a = i.distance.y;

						clip(col.a - _Cutoff);
						clip(i.distance.x < _Distance ? 1 : -1);
						return col;
					}

					ENDCG
				}
			}
		}
}
