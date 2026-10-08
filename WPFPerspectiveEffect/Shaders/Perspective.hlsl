sampler2D InputSampler : register(s0);

float M11 : register(c0);
float M12 : register(c1);
float M13 : register(c2);

float M21 : register(c3);
float M22 : register(c4);
float M23 : register(c5);

float M31 : register(c6);
float M32 : register(c7);
float M33 : register(c8);

struct PSInput
{
    float2 TexCoord : TEXCOORD0;
};

float4 main(PSInput input) : COLOR
{
    float x = input.TexCoord.x;
    float y = input.TexCoord.y;

    float sourceX = M11 * x + M12 * y + M13;

    float sourceY = M21 * x + M22 * y + M23;

    float w = M31 * x + M32 * y + M33;

    if (abs(w) < 0.00001)
        return float4(0, 0, 0, 0);

    float2 uv = float2(sourceX / w, sourceY / w);

    if (uv.x < 0.0 ||
        uv.x > 1.0 ||
        uv.y < 0.0 ||
        uv.y > 1.0)
    {
        return float4(0, 0, 0, 0);
    }

    return tex2D(InputSampler, uv);
}