Shader "RogueGods/Unlit/Ghost"
{
    Properties
    {
        [HDR]_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _FresnelExponent("Fresnel Exponent", float) = 2
        _GhostIntensity("Ghost Intensity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "SimpleLit" "IgnoreProjector" = "True" "ShaderModel"="2.0"}
        LOD 300
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite on
        Cull back
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            HLSLPROGRAM
            
            #pragma vertex vert
			#pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            CBUFFER_START(UnityPerMaterial)
            half4 _FresnelColor;
            float  _FresnelExponent;
            float _GhostIntensity;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
               
                UNITY_VERTEX_INPUT_INSTANCE_ID                
            };

            struct Varyings
            {
                float4 vertex : SV_POSITION;
                float3 normalWS : NORMAL;
                float3 viewDirWS : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
               UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;

                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                return output;
            }

            float calculateFresnel(float3 Normal, float3 ViewDir, float Power)
            {
                return pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            half4 frag(Varyings input) : SV_Target
            {
               half4 finaColor;
               const float fresnel = calculateFresnel(input.normalWS, input.viewDirWS, _FresnelExponent);
               finaColor.rgb = fresnel * _FresnelColor.rgb;
               finaColor.a = fresnel;
               finaColor *= _GhostIntensity;
               return finaColor;
            }

            ENDHLSL
        }
    }
    FallBack "Diffuse"
}