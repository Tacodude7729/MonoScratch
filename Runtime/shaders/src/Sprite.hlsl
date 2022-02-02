#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float BrightnessEffect;

static const float epsilon = 1e-3;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 pixelColor = tex2D(SpriteTextureSampler,input.TextureCoordinates);

	pixelColor.rgb = clamp(pixelColor.rgb + float3(BrightnessEffect, BrightnessEffect, BrightnessEffect), float3(0, 0, 0), float3(1, 1, 1));
	pixelColor.rgb = clamp(pixelColor.rgb / (pixelColor.a + epsilon), 0.0, 1.0);

	return pixelColor;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
	}
};