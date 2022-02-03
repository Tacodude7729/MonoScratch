#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float2 CanvasSize;

struct VertexShaderInput {
	float2 Position : SV_POSITION;

    float2 LinePoint : TEXCOORD0;
    float2 LinePointDiff : TEXCOORD1;

    float4 LineColor : COLOR;
    float2 LineLengthThickness : TEXCOORD2;
};

struct VertexShaderOutput {
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;

	float4 LineColor : COLOR;
	float2 LineLengthThickness : TEXCOORD1;
};

static const float epsilon = 1e-3;

VertexShaderOutput MainVS(VertexShaderInput input) {
	VertexShaderOutput output = (VertexShaderOutput)0;

	float2 position = input.Position;
	float expandedRadius = (input.LineLengthThickness.y * 0.5) + 1.4142135623730951;

	output.TexCoord.x = lerp(0.0, input.LineLengthThickness.x + (expandedRadius * 2.0), input.Position.x) - expandedRadius;
	output.TexCoord.y = ((input.Position.y - 0.5) * expandedRadius) + 0.5;

	position.x *= input.LineLengthThickness.x + (2.0 * expandedRadius);
	position.y *= 2.0 * expandedRadius;

	position -= expandedRadius;

	float2 pointDiff = input.LinePointDiff;

	pointDiff.x = (abs(pointDiff.x) < epsilon && abs(pointDiff.y) < epsilon) ? epsilon : pointDiff.x;

	float2 normalized = pointDiff / max(input.LineLengthThickness.x, epsilon);
	position = mul(float2x2(normalized.x, normalized.y, -normalized.y, normalized.x), position);

	position += input.LinePoint;
	position *= 2.0 / CanvasSize;

	output.Position = float4(position, 0, 1);

	output.LineLengthThickness = input.LineLengthThickness;
	output.LineColor = input.LineColor;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR {
	float d = ((input.TexCoord.x - clamp(input.TexCoord.x, 0.0, input.LineLengthThickness.x)) * 0.5) + 0.5;

	float ln = distance(float2(0.5, 0.5), float2(d, input.TexCoord.y)) * 2.0;
	ln -= ((input.LineLengthThickness.y - 1.0) * 0.5);

	return input.LineColor * clamp(1.0 - ln, 0.0, 1.0);
}

technique LineDrawing {
	pass P0 {
        VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
		AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
	}
};