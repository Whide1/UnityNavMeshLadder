Shader "Dev/Worldspace Scaler" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Scale ("Texture Scale", Float) = 1.0
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert

sampler2D _MainTex;
fixed4 _Color;
float _Scale;

struct Input
{
	float3 worldNormal;
	float3 worldPos;
};

void surf (Input IN, inout SurfaceOutput o)
{
	float2 UV;
	fixed4 c;

	if(abs(IN.worldNormal.x)>0.5)
	{
		UV = IN.worldPos.yz;
		c = tex2D(_MainTex, UV* _Scale);
	}
	else if(abs(IN.worldNormal.y)>0.5)
	{
		UV = IN.worldPos.xz;
		c = tex2D(_MainTex, UV* _Scale);
	}
	else
	{
		UV = IN.worldPos.xy;
		c = tex2D(_MainTex, UV* _Scale);
	}

	o.Albedo = c.rgb * _Color;
}
ENDCG
}

Fallback "VertexLit"
}
