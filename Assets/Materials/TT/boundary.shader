Shader "Custom/boundary"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_BGColor("BGColor", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 4.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float visibility;
			fixed4 color;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _BGColor;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

		float3 Overlay(float4 _color, float4 _color2) {
			float remainA1 = 1.0 - _color.a;

			float3 r = _color.rgb * _color.a;
			float3 viewCol = remainA1 * _color2.rgb;

			return r + viewCol;
		}

		void vert(inout appdata_full b, out Input data) {
			UNITY_INITIALIZE_OUTPUT(Input, data);

			float3 cameraPos = UnityObjectToViewPos(b.vertex);
			
			float3 pos = mul(unity_ObjectToWorld, b.vertex);

			pos = pos - float3(1, 0, 1);

			float len = dot(pos, pos);
			data.visibility = max(0, len - 100);
			data.visibility = min(1, data.visibility / 100);


		}

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed4 cc = _BGColor;
			cc.a = IN.visibility;


            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			fixed3 ccc = Overlay(cc, c);
            o.Albedo = ccc.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
