Shader "XLCW/Grass" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Dir("Direction", vector) = (1,0,0,0)
		_Power("_Power",float) = 1.0
		_TimeOff("_TimeOffset",float)=0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector"="True"}
		LOD 200
		
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert noforwardadd

		

		sampler2D _MainTex;
		float3 _Dir;
		half _Power;
		half _TimeOff;

		struct Input {
			float2 uv_MainTex;
		};

		void vert(inout appdata_full v)
		{
			v.vertex.xyz += _Dir * _Power*sin(_Time.y+_TimeOff)*v.texcoord.y;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
