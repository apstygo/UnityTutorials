using System;
using UnityEngine;

public class GPUGraph: MonoBehaviour {
    [SerializeField]
    ComputeShader computeShader;

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;

    [SerializeField, Range(10, maxResolution)]
    int resolution;

    [SerializeField, Min(0f)]
    float functionDuration = 1f;

    [SerializeField, Min(0f)]
    float transitionDuration = 0.5f;

    private const int maxResolution = 1000;

    private ComputeBuffer positionBuffer;
    private static readonly int positionsId = Shader.PropertyToID("_Positions");
    private static readonly int resolutionId = Shader.PropertyToID("_Resolution");
    private static readonly int stepId = Shader.PropertyToID("_Step");
    private static readonly int timeId = Shader.PropertyToID("_Time");

    private Vector3 scale {
        get { return Vector3.one * 2f / resolution; }
    }

    void OnEnable() {
        positionBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4); // 3 floats 4 bytes each
    }

    void OnDisable() {
        positionBuffer.Release();
        positionBuffer = null;
    }

    void Update() {
        UpdateFunctionOnGPU();
    }

    private void UpdateFunctionOnGPU() {
        float step = 2f / resolution;

        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        computeShader.SetBuffer(0, positionsId, positionBuffer);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(0, groups, groups, 1);

        material.SetBuffer(positionsId, positionBuffer);
        material.SetFloat(stepId, step);

        // 2f / resolution accounts for half the size of a cube
        // sticking out of bounds
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));

        RenderParams rparams = new(material) {
            worldBounds = bounds,
            shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
            receiveShadows = true
        };

        Graphics.RenderMeshPrimitives(rparams, mesh, 0, resolution * resolution);
    }
}
