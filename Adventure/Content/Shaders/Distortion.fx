#if OPENGL
#define PS_SHADERMODEL ps_3_0
#else
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler Sampler;
uniform float2 Resolution;
uniform float4x4 View;
uniform float2 Center;
uniform float Radius;
uniform float Force;
uniform float Thickness;

float4 PSMain(float2 uv : TEXCOORD0) : COLOR0
{
    float ratio = Resolution.x / Resolution.y;
    float2 centerViewCoords = mul(float4(Center, 0.0, 1.0), View).xy / Resolution.x;    
    float distance = length(float2(uv.x, uv.y / ratio) - centerViewCoords);
    
    // Outer circle
    float mask = (1.0 - smoothstep(Radius - 0.1, Radius, distance));
    
    // Inner circle
    mask *= (smoothstep(Radius - Thickness - 0.1, Radius - Thickness, distance));
    
    // Displacement
    float2 disp = normalize(distance) * Force * mask;
    float2 coords = uv - disp * 0.001;
    
    // Color output
    float4 color = tex2D(Sampler, coords);
    
    // Draw Mask
    //color.rgb = float3(mask, mask, mask);
    
    return color;
}

technique Techninque1
{
    pass Pass1
    {
        PixelShader = compile PS_SHADERMODEL PSMain();
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
    }
};