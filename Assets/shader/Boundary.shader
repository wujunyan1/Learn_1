Shader "Custom/Boundary"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        //_Metallic ("Metallic", Range(0,1)) = 0.0
		_Specular("Specular", Color) = (0.2, 0.2, 0.2)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent+5" }
        LOD 200
		Offset -1, -1

        CGPROGRAM
        #pragma surface surf StandardSpecular alpha vertex:vert

        #pragma target 3.5

		#pragma multi_compile _ GRID_ON
		#pragma multi_compile _ HEX_MAP_EDIT_MODE
		#pragma shader_feature SHOW_MAP_DATA
		#include "HexMetrics.cginc"
		#include "HexCellData.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float4 visibility;
			float4 campColor;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed3 _Specular;

		void vert(inout appdata_full v, out Input data) {
			UNITY_INITIALIZE_OUTPUT(Input, data);
			//data.terrain = v.texcoord2.xyz;

			float4 cell0 = GetCellData(v, 0);
			float4 cell1 = GetCellData(v, 1);
			float4 cell2 = GetCellData(v, 2);

			data.visibility.x = cell0.x;
			data.visibility.y = cell1.x;
			data.visibility.z = cell2.x;
			//data.visibility = lerp(0.25, 1, data.visibility);
			data.visibility.xyz = lerp(0.25, 1, data.visibility.xyz);
			data.visibility.w = cell0.y * v.color.x + cell1.y * v.color.y + cell2.y * v.color.z;


			float4 cellCamp0 = GetCellCampData(v, 0);
			float4 cellCamp1 = GetCellCampData(v, 1);
			float4 cellCamp2 = GetCellCampData(v, 2);


			float campA = step(cellCamp0.a, cellCamp1.a);
			float4 camp01 = campA * cellCamp1 + (1 - campA) * cellCamp0;

			float campB = step(camp01.a, cellCamp2.a);
			data.campColor = campB * cellCamp2 + (1 - campB) * camp01;
		}

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
			float explored = IN.visibility.w;

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			//float3 co = Overlay(IN.campColor, c);

            o.Albedo = IN.campColor.rgb * _Color * explored; // c.rgb;
			o.Specular = _Specular * explored;
			o.Occlusion = explored;
            o.Smoothness = _Glossiness;
            o.Alpha = IN.campColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
