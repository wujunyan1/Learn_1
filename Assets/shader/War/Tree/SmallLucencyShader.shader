Shader "Custom/Lucency/SmallLucencyShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_AlphaLen("Alpha Len",Range(0,1000)) = 100
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1"  }
			Blend SrcAlpha One	// 混合模式
			Zwrite Off
			LOD 200

			CGPROGRAM
			// Upgrade NOTE: excluded shader from DX11; has structs without semantics (struct v2f members visibility)
			// #pragma exclude_renderers d3d11
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard alpha vertex:vert
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;

			struct Input
			{
				float2 uv_MainTex;
				float visibility;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			float _AlphaLen;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)


			void vert(inout appdata_full b, out Input data) {
				UNITY_INITIALIZE_OUTPUT(Input, data);

				float3 cameraPos = UnityObjectToViewPos(b.vertex);
				float len = dot(cameraPos, cameraPos);
				data.visibility = max(0, len - _AlphaLen);
				data.visibility = min(1, data.visibility / 100);
			}

			void surf (Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				//fixed4 c = _Color;
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = IN.visibility;
			}

			ENDCG
    }
    FallBack "Diffuse"
}
