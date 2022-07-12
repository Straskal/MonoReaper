#if OPENGL
#define PS_SHADERMODEL ps_3_0
#else
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Render texture sampler
sampler _sampler;

// Texture resolution
uniform float2 resolution;

// View matrix
uniform float4x4 view;

// Shockwave center in world coordinates
uniform float2 center;

// Shockwave radius
uniform float radius;

// Shockwave force
uniform float force;

// Shockwave thickness
uniform float thickness;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{	   
    float ratio = resolution.x / resolution.y;
    
    float4 center4 = float4(center, 0.0, 1.0);
    float2 centerViewCoords = mul(center4, view).xy / resolution.x;
    
    float distance = length(float2(uv.x, uv.y / ratio) - centerViewCoords);
    
    // Outer circle
    float mask = (1.0 - smoothstep(radius - 0.1, radius, distance));
    
    // Inner circle
    mask *= (smoothstep(radius - thickness - 0.1, radius - thickness, distance));
    
    // Displacement
    float2 disp = normalize(distance) * force * mask;
    float2 coords = uv - disp * 0.001;
    
    // Color output
    float4 color = tex2D(_sampler, coords);
    
    //color.r = tex2D(_sampler, coords + float2(0.002, 0) * force * mask * 0.01).r;
    //color.g = tex2D(_sampler, coords - 0.002 * force * mask * 0.01).g;
    //color.b = tex2D(_sampler, coords - float2(0.002, 0) * force * mask * 0.01).b;
    
    // Draw Mask
    //color.rgb = float3(mask, mask, mask);
    
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