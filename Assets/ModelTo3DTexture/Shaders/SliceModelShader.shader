Shader "Unlit/SliceModelShader"
{
    Properties
    {
		_facadeColor("FacadeColor", Color) = (1,1,1,1)
		_backColor("BackColor", Color) = (1,1,1,1)
		_height("Height", Range(0,2)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}

        Pass
        {
			Cull Off
			HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

			float _height;
			float4 _backColor, _facadeColor;
			float4 _SliceObjectPos;

            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.worldPos = worldPos;

                return o;
            }

            float4 frag (v2f i, float facing : VFACE) : SV_Target
            {
				float4 heightRet = step(2, i.worldPos.y + _height);
				float4 facade = heightRet * _facadeColor;
				float4 back = heightRet * _backColor;
				float4 finalColor = 0;

				finalColor = lerp(facade, back, facing > 0);

				clip(finalColor.a - 0.5);

				return finalColor;
            }
            ENDHLSL
        }
    }
}
