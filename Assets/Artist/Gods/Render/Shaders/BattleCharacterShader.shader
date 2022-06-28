Shader "RogueGods/BattleCharacter"
{
    Properties
    {
        [Header(Basic Setting)]
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        
        /* 沟通考虑战斗中不使用任何alpha，故而将高光贴图合并到albedo.a  --2021-11-24 snow
        [Toggle(_ALPHATEST_ON)] _ALPHATEST_ON("Enable Alpha Test", int ) = 0
        _Cutoff("Alpha Clipping", Range(0.0, 1.0)) = 0.5
        */
        
        [Toggle(_NORMAL)] _NormalEnabled("Enable Normal", Int) = 0
        _BumpScale("Scale", Float) = 1.0
        [Normal]_BumpMap("Normal Map", 2D) = "bump" {}
        
        _LambertFactor("LambertFactor", Range(0,1)) = 0.95

        [Space(30)]
        [Header(Ramp Setting)]
        _RampMap("Ramp" , 2D) = "white" {}
        _RampScale("RampScale", Float) = 1.0
        _RampRotation("RampRotate", Float) = 1.0
        
        [Space(10)]
        _SpecularCol("SpecularCol", Color) = (1,1,1,1)
        _SpecularIntense("SpecularIntense", Float) = 0.0
        _SpecularThreshold("SpecularThreshold", Range(0,1)) = 0.9
        [Space(10)]

        [Space(30)]
        [Header(Emission Setting)]
        _EmissionMap("EmissionMap", 2D) = "white" {}
        [Toggle(_EMISSION)] _EmissionEnabled("Enable Emission", Int) = 0
        [HDR] _EmissionColor("Color", Color) = (0,0,0)

        [Space(30)]
        [Header(Outline Setting)]
        _Outline("OutlineWidth", Float) = 0
        _OutlineColor("OutlineColor", Color) = (0,0,0)

        [Space(30)]
        [Header(Fresnel Setting)]
        [Toggle(_FRESNEL)] _FresnelEnabled("Enable Fresnel", Int) = 0
        [HDR]_FresnelColor("Fresnel", Color) = (1,1,1,1)
        _FresnelExponent("Fresnel Exponent", float) = 2
        
        [Space(30)]
        [Header(Battle Setting)]
        _Direction("Dash Direction", vector) = (0,0,0,1)
        _WarningColor("WarningColor", Color) = (1,1,1,1) // 警戒颜色
        _WarningIntensity("WarningIntensity", Range(0,1)) = 0 // 警戒强度

        //留给shadow
        [HideInInspector] _Cull("__cull", Float) = 2.0

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        // ObsoleteProperties
        [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
        [HideInInspector] _Color("Base Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _Shininess("Smoothness", Float) = 0.0
        [HideInInspector] _GlossinessSource("GlossinessSource", Float) = 0.0
        [HideInInspector] _SpecSource("SpecularHighlights", Float) = 0.0
    }
    SubShader
    {
        //Forward
        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
                "RenderType"="Opaque"
                "Queue"="Geometry"
            }
            Cull Back
            Blend One Zero
            ZTest LEqual
            ZWrite On

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            //必须生成变体的关键字
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            //依据使用情况生成变体的       
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _NORMAL
            #pragma shader_feature_local_fragment _FRESNEL
            #pragma shader_feature_local_fragment _ALPHATEST_ON


            Texture2D _BaseMap;
            Texture2D _BumpMap;
            Texture2D _RampMap;
            Texture2D _EmissionMap;

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _BumpMap_ST;
            float4 _EmissionMap_ST;
            float4 _RampMap_ST;
            float _Cutoff;

            half4 _Direction;

            float4 _BaseColor;
            float4 _SpecularCol;
            float _LambertFactor;
            float _BumpScale;
            float _RampScale;
            float _RampRotation;
            float4 _EmissionColor;
            half _SpecularThreshold;
            half _SpecularIntense;
            half4 _WarningColor;
            half _WarningIntensity;
            half4 _FresnelColor;
            float  _FresnelExponent;
            CBUFFER_END

            SamplerState sampler_BaseMap;
            SamplerState sampler_BumpMap;
            SamplerState sampler_RampMap;
            SamplerState sampler_EmissionMap;

            struct VertexInput
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct fragData
            {
                float2 uv : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);

                float3 positionWS : TEXCOORD2;
                float3 normalWS : TEXCOORD3;
                float4 tangentWS : TEXCOORD4;
                float3 viewDirWS : TEXCOORD5;
                half4 fogFactorAndVertexLight : TEXCOORD6; // x: fogFactor, yzw: vertex light
                float4 shadowCoord : TEXCOORD7;
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            half3 SampleEmission(float2 uv, half3 emissionColor, TEXTURE2D_PARAM(emissionMap, sampler_emissionMap))
            {
                return SAMPLE_TEXTURE2D(emissionMap, sampler_emissionMap, uv).rgb * emissionColor;
            }

            half3 lambertLighting(Light l, half3 normalWS, half4 shadowMask)
            {
                half3 lCol = 0;
                half NdotL = saturate(dot(normalWS, l.direction));
                half attenuation = NdotL * l.distanceAttenuation * l.shadowAttenuation;
                lCol = l.color * attenuation;
                return lCol;
            }

            half3 lambertLightingByProperty(half3 lightColor, half3 lightDir, half3 normalWS)
            {
                half3 lCol = 0;
                half NdotL = saturate(dot(normalWS, lightDir));
                lCol = lightColor * NdotL;
                return lCol;
            }

            float calculateFresnel(float3 Normal, float3 ViewDir, float Power)
            {
                return pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }

            fragData vert(VertexInput inData)
            {
                fragData outData = (fragData)0;
                UNITY_SETUP_INSTANCE_ID(inData);
                UNITY_TRANSFER_INSTANCE_ID(inData, outData);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(outData);

                outData.uv = inData.texcoord;

                //inData.positionOS += _Direction;

                VertexPositionInputs vertexPosInput = GetVertexPositionInputs(inData.positionOS.xyz);


                // 面法线
                half3 normalWS = TransformObjectToWorldNormal(inData.normalOS);
                real sign = inData.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(TransformObjectToWorldDir(inData.tangentOS.xyz), sign);
                outData.tangentWS = tangentWS;

                //视线
                half3 viewDirWS = GetWorldSpaceViewDir(vertexPosInput.positionWS);
                half3 vertexLight = VertexLighting(vertexPosInput.positionWS, normalWS);
                outData.normalWS = normalWS;
                outData.viewDirWS = viewDirWS;

                //取GI信息
                half fogFactor = ComputeFogFactor(vertexPosInput.positionCS.z);
                OUTPUT_SH(outData.normalWS.xyz, outData.vertexSH);
                outData.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                //世界坐标、阴影坐标和屏幕坐标
                outData.shadowCoord = GetShadowCoord(vertexPosInput);

                float3 wPos = vertexPosInput.positionWS;
                half NDotD = max(0, dot(normalWS, _Direction));
                float noise = frac(sin(dot(inData.texcoord.xy, float2(12.9898, 78.233))) * 43758.5453);
                // 这个noise抄的一个常见的随机噪波通用公式
                wPos += _Direction.xyz * _Direction.w * noise * NDotD;
                outData.positionWS = wPos.xzy;
                outData.positionCS = mul(UNITY_MATRIX_VP, float4(wPos, 1));


                return outData;
            }

            float4 frag(fragData inData) : SV_Target
            {

                float4 col = 0;
                half2 mainUV = TRANSFORM_TEX(inData.uv, _BaseMap);
                //取固有色
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, mainUV);
                albedo.rgb = albedo.rgb * _BaseColor.rgb;
                /* 沟通考虑战斗中不使用任何alpha，故而将高光贴图合并到albedo.a  --2021-11-24 snow
                #if defined(_ALPHATEST_ON)
                 clip(albedo.a - _Cutoff);
                #endif
                */
                //获取法线
                half3 normalWS = 1;
                #ifndef _NORMAL
                normalWS = inData.normalWS;
                #else//从发现贴图取
                    half2 bumpUV = TRANSFORM_TEX(inData.uv, _BumpMap);
                    half4 normalVal = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, bumpUV);
                    half3 normalTS = UnpackNormalScale(normalVal, _BumpScale);
                    float sgn = inData.tangentWS.w;      // should be either +1 or -1
                    float3 bitangent = sgn * cross(inData.normalWS.xyz, inData.tangentWS.xyz);
                    normalWS = TransformTangentToWorld(normalTS, half3x3(inData.tangentWS.xyz, bitangent, inData.normalWS.xyz));
                #endif

                //  计算主光源
                half3 lightCol = 0;
                RogueGodsLight mainLight = GetCharacterMainLight();
                half3 mainLightCol = mainLight.color;
                //float3(_SceneLighting._11, _SceneLighting._12,_SceneLighting._13)*_SceneLighting._14;
                float3 mainLightDir = mainLight.direction;
                //float3(_SceneLightingDir._11, _SceneLightingDir._12, _SceneLightingDir._13);
                half3 halfLambert = dot(normalWS, mainLightDir) * 0.5 + 0.5;
                lightCol += mainLightCol * halfLambert;

                // 用明度取ramp， ramp只由灯光方向决定
                half brightIntense = saturate(dot(normalWS, mainLightDir));
                half2 rampUV = half2(brightIntense, sin((mainUV.x + mainUV.y * _RampRotation) * _RampScale));
                half4 rampVal = SAMPLE_TEXTURE2D(_RampMap, sampler_RampMap, rampUV);
                lightCol = lightCol * rampVal.r;

                half3 addLightCol = 0;
                half3 addLightDir = 0;
                // 计算辅助光
                RogueGodsLight additionalLight = GetCharacterAdditionalLight();
                addLightCol = additionalLight.color;
                addLightDir = additionalLight.direction;
                halfLambert = dot(normalWS, addLightDir) * 0.5 + 0.5;
                lightCol += addLightCol * halfLambert;

                //计算GI
                half3 bakedGI = SampleSHPixel(inData.vertexSH, normalWS);
                lightCol += bakedGI;

                //主色强度分层处理
                col.rgb = albedo.rgb * _LambertFactor * lightCol + (1.0 - _LambertFactor) * albedo.rgb;

                //计算高光部分
                //Emission和高光一块儿计算
                half4 specularCol = _SpecularCol * albedo.a;
                half specularVal = step(_SpecularThreshold, brightIntense);
                specularCol.rgb = specularCol * lightCol;
                col.rgb += col.rgb *  specularCol * specularVal * (1 + _SpecularIntense);

                //取自发光
                #ifdef _EMISSION
                    half2 emissionUV = TRANSFORM_TEX(inData.uv, _EmissionMap);
                    half3 emissionMapColor = SampleEmission(emissionUV, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
                    half3 emissionCol = emissionMapColor.rgb * _EmissionColor;
                    col.rgb += emissionCol;
                #endif
                //col.a = albedo.a;
                // changed by rui
                // 战斗角色暂时不支持alpha

                col.rgb = lerp(col.rgb, _WarningColor.rgb, _WarningIntensity);
                col.a = 1.0;

                #ifdef _FRESNEL
                float fresnel = calculateFresnel(normalWS, inData.viewDirWS, _FresnelExponent);
                col.rgb += fresnel * _FresnelColor;
                #endif
                
                return col;
            }
            ENDHLSL
        }

        Pass
        {
            Name "PlanarShadow"
            Tags
            {
                "LightMode" = "PlanarShadow"
            }

            //用使用模板测试以保证alpha显示正确
            Stencil
            {
                Ref 0
                Comp equal
                Pass incrWrap
                Fail keep
                ZFail keep
            }

            Cull Off

            //透明混合模式
            Blend SrcAlpha OneMinusSrcAlpha

            //关闭深度写入
            ZWrite off

            //深度稍微偏移防止阴影与地面穿插
            Offset -1 , 0

            HLSLPROGRAM
            #pragma shader_feature _CLIPPING
            #pragma shader_feature _ALPHATEST_ON
            #pragma shader_feature _ALPHAPREMULTIPLY_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/PlanarShadow.hlsl"

            #pragma vertex PlanarShadowVertex
            #pragma fragment PlanarShadowFragment
            ENDHLSL
        }

        // Outline
        Pass
        {
            Name "Outline"
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 color : COLOR;
            };

            struct fragData
            {
                float4 pos : SV_POSITION;
                half4 color : COLOR;
            };

            uniform float _Outline;
            uniform float4 _OutlineColor;

            fragData vert(Attributes input)
            {
                fragData output;
                float3 vNormal = mul((float3x3)UNITY_MATRIX_IT_MV, input.normalOS);
                float4 vPos = mul(UNITY_MATRIX_MV, input.positionOS);
                output.pos = mul(UNITY_MATRIX_P, vPos);
                float cameraDis = length(vPos.xyz);
                float fovTan = unity_CameraProjection[1].y;
                cameraDis = clamp(0, 20, cameraDis);
                float sufacey = -exp(-cameraDis * 0.1) + 1;
                float disEffect = 1 + cameraDis * (24.0 - sufacey * 7.5) * 0.05;
                float2 offset = mul((float2x2)UNITY_MATRIX_P, vNormal.xy);
                offset = normalize(offset);
                output.pos.xy += offset * _Outline * disEffect * input.color.a;
                output.color = float4(input.color.rgb, input.color.a);
                return output;
            }

            float4 frag(fragData input) : SV_Target
            {
                float4 col = float4(0, 0, 0, 0);
                clip(input.color.a - 0.25);
                col.rgb = input.color.rgb * _OutlineColor.rgb;
                col.rgb = saturate(col.rgb);
                col.a = _OutlineColor.a;
                return col;
            }

            ENDCG
        }

        // This pass is used when drawing to a _CameraNormalsTexture texture
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            ZWrite On
            Cull back

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _GLOSSINESS_FROM_BASE_ALPHA

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/SimpleLitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Hidden/Shader Graph/FallbackError"
    //CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.BattleCharacterShader"
}