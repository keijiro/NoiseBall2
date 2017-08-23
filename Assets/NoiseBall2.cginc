#include "UnityCG.cginc"

#if SHADER_TARGET >= 45 && (defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE) || defined(SHADER_API_XBOXONE) || defined(SHADER_API_PSSL) || defined(SHADER_API_SWITCH) || defined(SHADER_API_VULKAN) || (defined(SHADER_API_METAL) && defined(UNITY_COMPILER_HLSLCC)))
    #define NOISEBALL2_ENABLE
#endif

float4x4 _LocalToWorld;
half3 _Color;

struct Varyings
{
    float4 position : SV_POSITION;
    half3 color : COLOR;
};

#ifdef NOISEBALL2_ENABLE
StructuredBuffer<float4> _PositionBuffer;
StructuredBuffer<float4> _NormalBuffer;
#endif

Varyings Vertex(uint vertexID : SV_VertexID)
{
    float4 vp = float4(0, 0, 0, 1);
    half3 vc = 0;

#ifdef NOISEBALL2_ENABLE
    float3 p = _PositionBuffer[vertexID].xyz;
    float3 n = _NormalBuffer[vertexID].xyz;

    float4 wp = mul(_LocalToWorld, float4(p, 1));
    float3 wn = normalize(mul((float3x3)_LocalToWorld, n));

    vp = UnityWorldToClipPos(wp);
    vc = saturate((length(p) - 1) / 3) * abs(wn.y);
#endif

    Varyings o;
    o.position = vp;
    o.color = vc * _Color;
    return o;
}

half4 Fragment(Varyings input) : SV_Target
{
    return half4(input.color, 1);
}
