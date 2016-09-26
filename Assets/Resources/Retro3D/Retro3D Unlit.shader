// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Retro3D/Unlit"
{
    Properties
    {
        _MainTex("Base", 2D) = "white" {}
        _Color("Color", Color) = (0.5, 0.5, 0.5, 1)
        _GeoRes("Geometric Resolution", Float) = 40
    }
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
                float3 texcoord : TEXCOORD;
				fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _GeoRes;
			float3 pos;

            v2f vert(appdata_full v)
            {
                v2f o;

				pos = _WorldSpaceCameraPos;
				pos.y = mul(unity_ObjectToWorld, v.vertex).y;
				float dist = distance(floor(pos * 0.3) / 0.3, mul(unity_ObjectToWorld, v.vertex));
				float res = _GeoRes - (clamp(dist, 0, _GeoRes * 0.6));
                float4 wp = mul(UNITY_MATRIX_MV, v.vertex);
                wp.xyz = floor(wp.xyz * res) / res;

                float4 sp = mul(UNITY_MATRIX_P, wp);
                o.position = sp;

                float2 uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord = float3(uv * sp.w, sp.w);
				o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.texcoord.xy / i.texcoord.z;
				return tex2D(_MainTex, uv) * i.color;
            }

            ENDCG
        }
    }
}
