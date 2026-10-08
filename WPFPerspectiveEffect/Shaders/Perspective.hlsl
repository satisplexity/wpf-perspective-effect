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

float4 UvDerivatives : register(c9);

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

    float2 uv = float2(sourceX, sourceY) / w;

    // Производные преобразованных UV по входным x и y.
    float2 derivativeX = (float2(M11, M21) - uv * M31) / w;

    float2 derivativeY = (float2(M12, M22) - uv * M32) / w;

    // Изменение UV при смещении на один экранный пиксель.
    float2 screenDx =
        derivativeX * UvDerivatives.x +
        derivativeY * UvDerivatives.y;

    float2 screenDy =
        derivativeX * UvDerivatives.z +
        derivativeY * UvDerivatives.w;

    float2 pixelWidth = max(
        abs(screenDx) + abs(screenDy),
        float2(0.000001, 0.000001));

    // Расстояние до ближайшего края по каждой оси.
    // Внутри изображения положительное, снаружи отрицательное.
    float2 edgeDistance = min(uv, 1.0 - uv);

    // Приблизительное покрытие пикселя изображением.
    float2 coverage = saturate(
        edgeDistance / pixelWidth + 0.5);

    float alpha = coverage.x * coverage.y;

    float4 color = tex2D(InputSampler, saturate(uv));

    // Умножаем и RGB, и альфу: цвет в WPF premultiplied.
    return color * alpha;
}