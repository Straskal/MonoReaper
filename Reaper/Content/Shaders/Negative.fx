#if OPENGL
#define PS_SHADERMODEL ps_3_0
#else
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler inputTexture;

float4 PixelShaderFunction(float2 textureCoordinates : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(inputTexture, textureCoordinates);

	if (color.a) {
		color.rgb = 1 - color.rgb;
	}

	return color;
}

technique Techninque1
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
	}
};