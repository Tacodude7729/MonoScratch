#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float AspectRatio;

float2 LinePoint1;
float2 LinePoint2;
float LineThickness;

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
	// These 5 lines took me so long :_(
	float4 pixelColor = tex2D(SpriteTextureSampler,input.TextureCoordinates);
	input.TextureCoordinates.y *= AspectRatio;
	input.TextureCoordinates -= LinePoint1;
	float2 diff = LinePoint2 - LinePoint1;
	float dist = length(input.TextureCoordinates - diff * clamp(dot(input.TextureCoordinates, diff) / dot(diff, diff), 0.0, 1.0));
	return lerp(pixelColor, input.Color, smoothstep(LineThickness + 0.003, LineThickness, dist));
	// return lerp(pixelColor, input.Color, step(dist, LineThickness));
}

technique LineDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
	}
};