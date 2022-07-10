#if OPENGL
#define PS_SHADERMODEL ps_3_0
#else
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

// Render texture sampler
sampler _sampler;

// Shockwave center
uniform float2 center;

// Shockwave radius
uniform float radius;

// Shockwave force
uniform float force;

// Shockwave thickness
uniform float thickness;

float4 PixelShaderFunction(float2 uv : TEXCOORD0) : COLOR0
{	   
    float2 resolution = float2(640.0, 360.0);
    float ratio = resolution.x / resolution.y;
    
    float distance = length(float2(uv.x, uv.y / ratio) - center);
    
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
    
    // Uncomment to visualize mask
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