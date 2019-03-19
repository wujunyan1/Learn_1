// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:32761,y:32720,varname:node_2865,prsc:2|diff-8706-OUT,spec-3913-OUT,gloss-110-OUT,normal-7823-OUT,alpha-804-OUT;n:type:ShaderForge.SFN_TexCoord,id:8650,x:30968,y:32861,varname:node_8650,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:3768,x:32034,y:32364,varname:node_3768,prsc:2,spu:-0.1,spv:-0.05|UVIN-8650-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:7284,x:32073,y:32574,varname:_MainTex,prsc:2,tex:78f0fbcc80b95374f862bfa20bc0d918,ntxv:3,isnm:True|UVIN-8082-UVOUT,TEX-8178-TEX;n:type:ShaderForge.SFN_Panner,id:8082,x:31915,y:32574,varname:node_8082,prsc:2,spu:0.04,spv:0.05|UVIN-3526-OUT;n:type:ShaderForge.SFN_Vector3,id:3162,x:32073,y:32500,varname:node_3162,prsc:2,v1:-1,v2:-1,v3:1;n:type:ShaderForge.SFN_Multiply,id:4296,x:32249,y:32574,varname:node_4296,prsc:2|A-7284-RGB,B-3162-OUT;n:type:ShaderForge.SFN_Multiply,id:3526,x:31751,y:32574,varname:node_3526,prsc:2|A-8650-UVOUT,B-6608-OUT;n:type:ShaderForge.SFN_Vector1,id:6608,x:31613,y:32643,varname:node_6608,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Add,id:9258,x:32415,y:32574,varname:node_9258,prsc:2|A-8383-RGB,B-4296-OUT;n:type:ShaderForge.SFN_Tex2d,id:8383,x:32199,y:32364,varname:node_8383,prsc:2,tex:78f0fbcc80b95374f862bfa20bc0d918,ntxv:3,isnm:True|UVIN-3768-UVOUT,TEX-8178-TEX;n:type:ShaderForge.SFN_Vector1,id:8824,x:31508,y:32843,varname:node_8824,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Multiply,id:3610,x:31671,y:32778,varname:node_3610,prsc:2|A-8650-UVOUT,B-8824-OUT;n:type:ShaderForge.SFN_Panner,id:3577,x:31845,y:32778,varname:node_3577,prsc:2,spu:-0.03,spv:0.05|UVIN-3610-OUT;n:type:ShaderForge.SFN_Tex2d,id:6761,x:32029,y:32778,varname:node_6761,prsc:2,tex:78f0fbcc80b95374f862bfa20bc0d918,ntxv:3,isnm:True|UVIN-3577-UVOUT,TEX-8178-TEX;n:type:ShaderForge.SFN_Vector1,id:6043,x:31508,y:32997,varname:node_6043,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Multiply,id:648,x:31658,y:32922,varname:node_648,prsc:2|A-8650-UVOUT,B-6043-OUT;n:type:ShaderForge.SFN_Panner,id:9298,x:31845,y:32922,varname:node_9298,prsc:2,spu:0.05,spv:0|UVIN-648-OUT;n:type:ShaderForge.SFN_Tex2d,id:9594,x:32029,y:32922,varname:node_9594,prsc:2,tex:78f0fbcc80b95374f862bfa20bc0d918,ntxv:3,isnm:True|UVIN-9298-UVOUT,TEX-8178-TEX;n:type:ShaderForge.SFN_Add,id:5112,x:32198,y:32778,varname:node_5112,prsc:2|A-6761-RGB,B-9594-RGB;n:type:ShaderForge.SFN_Lerp,id:5744,x:32373,y:32778,varname:node_5744,prsc:2|A-9258-OUT,B-5112-OUT,T-7709-OUT;n:type:ShaderForge.SFN_Subtract,id:8786,x:31845,y:33070,varname:node_8786,prsc:2|A-8471-OUT,B-3007-OUT;n:type:ShaderForge.SFN_Vector1,id:3007,x:31643,y:33198,varname:node_3007,prsc:2,v1:5;n:type:ShaderForge.SFN_Depth,id:8471,x:31643,y:33070,varname:node_8471,prsc:2;n:type:ShaderForge.SFN_Divide,id:1917,x:32029,y:33050,varname:node_1917,prsc:2|A-8786-OUT,B-3993-OUT;n:type:ShaderForge.SFN_Vector1,id:3993,x:31845,y:33198,varname:node_3993,prsc:2,v1:10;n:type:ShaderForge.SFN_Clamp01,id:7709,x:32198,y:32922,varname:node_7709,prsc:2|IN-1917-OUT;n:type:ShaderForge.SFN_Lerp,id:7823,x:32531,y:32886,varname:node_7823,prsc:2|A-5744-OUT,B-4372-OUT,T-1473-OUT;n:type:ShaderForge.SFN_Multiply,id:1341,x:31643,y:33265,varname:node_1341,prsc:2|A-8650-UVOUT,B-3690-OUT;n:type:ShaderForge.SFN_Vector1,id:3690,x:31492,y:33198,varname:node_3690,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Panner,id:8226,x:31845,y:33255,varname:node_8226,prsc:2,spu:-0.03,spv:-0.09|UVIN-1341-OUT;n:type:ShaderForge.SFN_Tex2d,id:9337,x:32029,y:33223,varname:node_9337,prsc:2,tex:78f0fbcc80b95374f862bfa20bc0d918,ntxv:0,isnm:False|UVIN-8226-UVOUT,TEX-8178-TEX;n:type:ShaderForge.SFN_Tex2dAsset,id:8178,x:30968,y:32674,ptovrint:False,ptlb:node_8178,ptin:_node_8178,varname:node_8178,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:78f0fbcc80b95374f862bfa20bc0d918,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Append,id:8184,x:31474,y:33401,varname:node_8184,prsc:2|A-7281-OUT,B-9700-OUT;n:type:ShaderForge.SFN_Multiply,id:7281,x:31298,y:33401,varname:node_7281,prsc:2|A-8650-UVOUT,B-3992-OUT;n:type:ShaderForge.SFN_Vector1,id:3992,x:31282,y:33312,varname:node_3992,prsc:2,v1:0.08;n:type:ShaderForge.SFN_Vector1,id:945,x:31144,y:33596,varname:node_945,prsc:2,v1:0.04;n:type:ShaderForge.SFN_Multiply,id:9700,x:31298,y:33525,varname:node_9700,prsc:2|A-8650-UVOUT,B-945-OUT;n:type:ShaderForge.SFN_Panner,id:7863,x:31643,y:33401,varname:node_7863,prsc:2,spu:0.03,spv:0|UVIN-8184-OUT;n:type:ShaderForge.SFN_Vector3,id:2950,x:32198,y:33350,varname:node_2950,prsc:2,v1:-1,v2:-1,v3:1;n:type:ShaderForge.SFN_Multiply,id:863,x:32198,y:33223,varname:node_863,prsc:2|A-3496-RGB,B-2950-OUT;n:type:ShaderForge.SFN_Tex2d,id:3496,x:31833,y:33401,varname:node_3496,prsc:2,tex:78f0fbcc80b95374f862bfa20bc0d918,ntxv:0,isnm:False|UVIN-7863-UVOUT,TEX-8178-TEX;n:type:ShaderForge.SFN_Add,id:4372,x:32233,y:33050,varname:node_4372,prsc:2|A-9337-RGB,B-863-OUT;n:type:ShaderForge.SFN_Subtract,id:3635,x:32400,y:33263,varname:node_3635,prsc:2|A-8471-OUT,B-8641-OUT;n:type:ShaderForge.SFN_Vector1,id:8641,x:32362,y:33408,varname:node_8641,prsc:2,v1:5;n:type:ShaderForge.SFN_Divide,id:8925,x:32575,y:33201,varname:node_8925,prsc:2|A-3635-OUT,B-1574-OUT;n:type:ShaderForge.SFN_Vector1,id:1574,x:32348,y:33201,varname:node_1574,prsc:2,v1:10;n:type:ShaderForge.SFN_Clamp01,id:1473,x:32504,y:33034,varname:node_1473,prsc:2|IN-8925-OUT;n:type:ShaderForge.SFN_Color,id:2910,x:32415,y:32364,ptovrint:False,ptlb:MainColor,ptin:_MainColor,varname:node_2910,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5803922,c2:0.6196079,c3:0.6862745,c4:1;n:type:ShaderForge.SFN_Slider,id:3913,x:32565,y:32377,ptovrint:False,ptlb:Metallick,ptin:_Metallick,varname:node_3913,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:110,x:32565,y:32467,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_110,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7,max:1;n:type:ShaderForge.SFN_DepthBlend,id:804,x:32558,y:33338,varname:node_804,prsc:2;n:type:ShaderForge.SFN_DepthBlend,id:1008,x:32351,y:32080,varname:node_1008,prsc:2|DIST-1843-OUT;n:type:ShaderForge.SFN_Vector1,id:1843,x:32171,y:32080,varname:node_1843,prsc:2,v1:1.1;n:type:ShaderForge.SFN_Clamp01,id:9320,x:32527,y:32080,varname:node_9320,prsc:2|IN-1008-OUT;n:type:ShaderForge.SFN_Lerp,id:8706,x:32895,y:32298,varname:node_8706,prsc:2|A-2835-R,B-2910-RGB,T-9320-OUT;n:type:ShaderForge.SFN_Tex2d,id:2835,x:32538,y:32188,varname:node_2835,prsc:2,tex:78f0fbcc80b95374f862bfa20bc0d918,ntxv:0,isnm:False|UVIN-3404-UVOUT,TEX-8178-TEX;n:type:ShaderForge.SFN_Multiply,id:9100,x:32171,y:32194,varname:node_9100,prsc:2|A-8650-UVOUT,B-2356-OUT;n:type:ShaderForge.SFN_Vector1,id:2356,x:31983,y:32243,varname:node_2356,prsc:2,v1:3;n:type:ShaderForge.SFN_Panner,id:3404,x:32339,y:32194,varname:node_3404,prsc:2,spu:0.2,spv:0.2|UVIN-9100-OUT;proporder:8178-2910-3913-110;pass:END;sub:END;*/

Shader "Custom/lake2" {
    Properties {
        _node_8178 ("node_8178", 2D) = "bump" {}
        _MainColor ("MainColor", Color) = (0.5803922,0.6196079,0.6862745,1)
        _Metallick ("Metallick", Range(0, 1)) = 1
        _Gloss ("Gloss", Range(0, 1)) = 0.7
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _node_8178; uniform float4 _node_8178_ST;
            uniform float4 _MainColor;
            uniform float _Metallick;
            uniform float _Gloss;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                UNITY_FOG_COORDS(8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_6984 = _Time;
                float2 node_3768 = (i.uv0+node_6984.g*float2(-0.1,-0.05));
                float3 node_8383 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_3768, _node_8178)));
                float2 node_8082 = ((i.uv0*0.5)+node_6984.g*float2(0.04,0.05));
                float3 _MainTex = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_8082, _node_8178)));
                float2 node_3577 = ((i.uv0*0.3)+node_6984.g*float2(-0.03,0.05));
                float3 node_6761 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_3577, _node_8178)));
                float2 node_9298 = ((i.uv0*0.25)+node_6984.g*float2(0.05,0));
                float3 node_9594 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_9298, _node_8178)));
                float2 node_8226 = ((i.uv0*0.1)+node_6984.g*float2(-0.03,-0.09));
                float3 node_9337 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_8226, _node_8178)));
                float2 node_7863 = (float4((i.uv0*0.08),(i.uv0*0.04))+node_6984.g*float2(0.03,0));
                float3 node_3496 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_7863, _node_8178)));float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 normalLocal = lerp(lerp((node_8383.rgb+(_MainTex.rgb*float3(-1,-1,1))),(node_6761.rgb+node_9594.rgb),saturate(((partZ-5.0)/10.0))),(node_9337.rgb+(node_3496.rgb*float3(-1,-1,1))),saturate(((partZ-5.0)/10.0)));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallick;
                float specularMonochrome;
                float2 node_3404 = ((i.uv0*3.0)+node_6984.g*float2(0.2,0.2));
                float3 node_2835 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_3404, _node_8178)));
                float node_1008 = saturate((sceneZ-partZ)/1.1);
                float node_9320 = saturate(node_1008);
                float3 diffuseColor = lerp(float3(node_2835.r,node_2835.r,node_2835.r),_MainColor.rgb,node_9320); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,saturate((sceneZ-partZ)));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _node_8178; uniform float4 _node_8178_ST;
            uniform float4 _MainColor;
            uniform float _Metallick;
            uniform float _Gloss;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_4669 = _Time;
                float2 node_3768 = (i.uv0+node_4669.g*float2(-0.1,-0.05));
                float3 node_8383 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_3768, _node_8178)));
                float2 node_8082 = ((i.uv0*0.5)+node_4669.g*float2(0.04,0.05));
                float3 _MainTex = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_8082, _node_8178)));
                float2 node_3577 = ((i.uv0*0.3)+node_4669.g*float2(-0.03,0.05));
                float3 node_6761 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_3577, _node_8178)));
                float2 node_9298 = ((i.uv0*0.25)+node_4669.g*float2(0.05,0));
                float3 node_9594 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_9298, _node_8178)));
                float2 node_8226 = ((i.uv0*0.1)+node_4669.g*float2(-0.03,-0.09));
                float3 node_9337 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_8226, _node_8178)));
                float2 node_7863 = (float4((i.uv0*0.08),(i.uv0*0.04))+node_4669.g*float2(0.03,0));
                float3 node_3496 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_7863, _node_8178)));float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float3 normalLocal = lerp(lerp((node_8383.rgb+(_MainTex.rgb*float3(-1,-1,1))),(node_6761.rgb+node_9594.rgb),saturate(((partZ-5.0)/10.0))),(node_9337.rgb+(node_3496.rgb*float3(-1,-1,1))),saturate(((partZ-5.0)/10.0)));
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Gloss;
                float perceptualRoughness = 1.0 - _Gloss;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _Metallick;
                float specularMonochrome;
                float2 node_3404 = ((i.uv0*3.0)+node_4669.g*float2(0.2,0.2));
                float3 node_2835 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_3404, _node_8178)));
                float node_1008 = saturate((sceneZ-partZ)/1.1);
                float node_9320 = saturate(node_1008);
                float3 diffuseColor = lerp(float3(node_2835.r,node_2835.r,node_2835.r),_MainColor.rgb,node_9320); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * saturate((sceneZ-partZ)),0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform sampler2D _node_8178; uniform float4 _node_8178_ST;
            uniform float4 _MainColor;
            uniform float _Metallick;
            uniform float _Gloss;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float4 projPos : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 node_2388 = _Time;
                float2 node_3404 = ((i.uv0*3.0)+node_2388.g*float2(0.2,0.2));
                float3 node_2835 = UnpackNormal(tex2D(_node_8178,TRANSFORM_TEX(node_3404, _node_8178)));
                float node_1008 = saturate((sceneZ-partZ)/1.1);
                float node_9320 = saturate(node_1008);
                float3 diffColor = lerp(float3(node_2835.r,node_2835.r,node_2835.r),_MainColor.rgb,node_9320);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallick, specColor, specularMonochrome );
                float roughness = 1.0 - _Gloss;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
