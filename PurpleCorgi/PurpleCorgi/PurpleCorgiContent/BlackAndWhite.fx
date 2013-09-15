sampler s0;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0  
{  
	float4 color = tex2D(s0, coords); 

	float y = (0.299 * color.x) + (0.587 * color.y) + (0.114 * color.z);
  
	return float4(y, y, y, 1);  
}  

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
