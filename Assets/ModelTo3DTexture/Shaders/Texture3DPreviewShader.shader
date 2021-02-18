Shader "Unlit/Texture3DPreviewShader"
{
    Properties
    {
        _MainTex ("Texture", 3D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
			HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_instancing

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float3 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			TEXTURE3D(_MainTex);
			SAMPLER(sampler_MainTex);

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
                o.vertex = TransformObjectToHClip(v.vertex);
				float3 worldPos = TransformObjectToWorld(v.vertex);
				o.uv = float3(worldPos.xz + 0.5, (worldPos.y));
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = 0;

				col += SAMPLE_TEXTURE3D_LOD(_MainTex, sampler_MainTex, i.uv, 0);

				if (col.r <= 0.5) discard;
                return col;
            }
            ENDHLSL
        }
    }
}
