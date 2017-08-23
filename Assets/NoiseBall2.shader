Shader "Hidden/NoiseBall2"
{
    Properties
    {
        _Color("", Color) = (1, 1, 1)
    }
    SubShader
    {
        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma target 4.5
            #include "NoiseBall2.cginc"
            ENDCG
        }
    }
}
