Shader "Custom/HeartbreakGlowShader"
{
    Properties
    {
        _BaseColor ("Base Color (Crystal)", Color) = (0.15, 0.05, 0.1, 1)
        _GlowColor ("Crack Glow Color", Color) = (1, 0.1, 0.4, 1)
        _PulseSpeed ("Pulse Speed", Float) = 2.5
        _GlowIntensity ("Glow Intensity", Range(1, 10)) = 4.0
        _CrackFrequency ("Crack Frequency", Float) = 15.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 worldPos     : TEXCOORD1;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _GlowColor;
                float _PulseSpeed;
                float _GlowIntensity;
                float _CrackFrequency;
            CBUFFER_END

            // Fungsi matematika sederhana untuk generate pola retakan prosedural (Simplex-like Noise)
            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(hash(i + float2(0.0,0.0)), hash(i + float2(1.0,0.0)), u.x),
                            lerp(hash(i + float2(0.0,1.0)), hash(i + float2(1.0,1.0)), u.x), u.y);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = vertexInput.positionCS;
                output.uv = input.uv;
                output.worldPos = vertexInput.positionWS;
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                // 1. Buat efek denyut jantung (Heartbeat Pulse) menggunakan waktu sin
                float pulse = sin(_Time.y * _PulseSpeed);
                pulse = abs(pulse); // Mengubah -1 ke 1 menjadi pola detak 0 ke 1
                
                // 2. Generate pola retakan neon tajam berbasis posisi dunia agar tidak melar saat dinding diskala
                float2 uvNoise = input.worldPos.xy * _CrackFrequency;
                float n = noise(uvNoise * 1.5);
                
                // Efek garis retakan patah-patah tajam (Makin dekat ke 0.5 makin menyala)
                float cracks = 1.0 - abs(n - 0.5) * 15.0;
                cracks = saturate(cracks);
                
                // 3. Gabungkan retakan dengan denyut nadi neon
                float3 emission = _GlowColor.rgb * cracks * (1.0 + pulse * _GlowIntensity);
                
                // 4. Warna akhir: Kristal gelap digabung retakan neon menyala
                float3 finalColor = _BaseColor.rgb + emission;
                
                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
}