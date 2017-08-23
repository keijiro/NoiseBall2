using UnityEngine;

[ExecuteInEditMode]
public class NoiseBall2 : MonoBehaviour
{
    #region Exposed attributes

    [SerializeField] int _triangleCount = 100;

    public int triangleCount {
        get { return _triangleCount; }
        set { _triangleCount = value; }
    }

    [SerializeField] float _triangleExtent = 0.1f;

    public float triangleExtent {
        get { return _triangleExtent; }
        set { _triangleExtent = value; }
    }

    [SerializeField] float _shuffleSpeed = 4;

    public float shuffleSpeed {
        get { return _shuffleSpeed; }
        set { _shuffleSpeed = value; }
    }

    [SerializeField] float _noiseAmplitude = 1;

    public float noiseAmplitude {
        get { return _noiseAmplitude; }
        set { _noiseAmplitude = value; }
    }

    [SerializeField] float _noiseFrequency = 1;

    public float noiseFrequency {
        get { return _noiseFrequency; }
        set { _noiseFrequency = value; }
    }

    [SerializeField] Vector3 _noiseMotion = Vector3.up;

    public Vector3 noiseMotion {
        get { return _noiseMotion; }
        set { _noiseMotion = value; }
    }

    [SerializeField, ColorUsage(false)] Color _color = Color.white;

    public Color color {
        get { return _color; }
        set { _color = value; }
    }

    [SerializeField] int _randomSeed = 0;

    #endregion

    #region Hidden attributes

    [SerializeField, HideInInspector] ComputeShader _compute;
    [SerializeField, HideInInspector] Shader _shader;

    #endregion

    #region Private fields

    ComputeBuffer _drawArgsBuffer;
    ComputeBuffer _positionBuffer;
    ComputeBuffer _normalBuffer;
    Material _material;
    float _localTime;

    #endregion

    #region Compute configurations

    const int kThreadCount = 64;
    int ThreadGroupCount { get { return _triangleCount / kThreadCount; } }
    int TriangleCount { get { return kThreadCount * ThreadGroupCount; } }

    #endregion

    #region MonoBehaviour functions

    void OnValidate()
    {
        _triangleCount = Mathf.Max(kThreadCount, _triangleCount);
        _triangleExtent = Mathf.Max(0, _triangleExtent);
        _noiseFrequency = Mathf.Max(0, _noiseFrequency);
    }

    void OnDisable()
    {
        if (_positionBuffer != null)
        {
            _positionBuffer.Release();
            _positionBuffer = null;
        }

        if (_normalBuffer != null)
        {
            _normalBuffer.Release();
            _normalBuffer = null;
        }
    }

    void OnDestroy()
    {
        if (_material != null)
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }
    }

    void OnRenderObject()
    {
        var time = _localTime + _randomSeed * 89.27345f;

        // Allocate/Reallocate the compute buffers when it hasn't been
        // initialized or the triangle count was changed from the last frame.
        if (_positionBuffer == null || _positionBuffer.count != TriangleCount * 3)
        {
            if (_positionBuffer != null) _positionBuffer.Release();
            if (_normalBuffer != null) _normalBuffer.Release();

            _positionBuffer = new ComputeBuffer(TriangleCount * 3, 16);
            _normalBuffer = new ComputeBuffer(TriangleCount * 3, 16);
        }

        // Invoke the update compute kernel.
        var kernel = _compute.FindKernel("Update");

        _compute.SetFloat("Time", _shuffleSpeed * time);
        _compute.SetFloat("Extent", _triangleExtent);
        _compute.SetFloat("NoiseAmplitude", _noiseAmplitude);
        _compute.SetFloat("NoiseFrequency", _noiseFrequency);
        _compute.SetVector("NoiseOffset", _noiseMotion * time);

        _compute.SetBuffer(kernel, "PositionBuffer", _positionBuffer);
        _compute.SetBuffer(kernel, "NormalBuffer", _normalBuffer);

        _compute.Dispatch(kernel, ThreadGroupCount, 1, 1);

        if (_material == null)
        {
            _material = new Material(_shader);
            _material.hideFlags = HideFlags.DontSave;
        }

        _material.SetColor("_Color", _color);
        _material.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);
        _material.SetBuffer("_PositionBuffer", _positionBuffer);
        _material.SetBuffer("_NormalBuffer", _normalBuffer);

        _material.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Triangles, TriangleCount * 3);
    }

    void Update()
    {
        if (Application.isPlaying) _localTime += Time.deltaTime;
    }

    #endregion
}
