Shader "Dumpling67/VhsOverlay"
{
    Properties
    {
        _MainTex ("Source", 2D) = "white" {}
        _Intensity ("Intensity", Range(0,1)) = 0.35
        _Scanline ("Scanline", Range(0,1)) = 0.25
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "VhsOverlay"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float _Intensity;
            float _Scanline;

            struct Attr { float4 pos : POSITION; float2 uv : TEXCOORD0; };
            struct V2F { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            V2F Vert(Attr v)
            {
                V2F o;
                o.pos = TransformObjectToHClip(v.pos.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 Frag(V2F i) : SV_Target
            {
                float2 uv = i.uv;
                float n = frac(sin(dot(uv, float2(12.9898, 78.233)) + _Time.y) * 43758.5453);
                uv.x += (n - 0.5) * 0.004 * _Intensity;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                half scan = sin(uv.y * 800.0) * _Scanline * 0.08;
                col.rgb -= scan;
                col.r += _Intensity * 0.03;
                return col;
            }
            ENDHLSL
        }
    }
}
