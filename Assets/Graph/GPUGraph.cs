using System;
using Unity.VisualScripting;
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

    private FunctionLibrary.FunctionName? previousFunction = null;
    private FunctionLibrary.FunctionName currentFunction = FunctionLibrary.FunctionName.Wave;
    private float elapsedTime;
    private ComputeBuffer positionBuffer;
    private static readonly int positionsId = Shader.PropertyToID("_Positions");
    private static readonly int resolutionId = Shader.PropertyToID("_Resolution");
    private static readonly int stepId = Shader.PropertyToID("_Step");
    private static readonly int timeId = Shader.PropertyToID("_Time");

    private Vector3 scale {
        get { return Vector3.one * 2f / resolution; }
    }

    private bool isTransitioning {
        get { return previousFunction.HasValue; }
    }

    void OnEnable() {
        positionBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4); // 3 floats 4 bytes each
    }

    void OnDisable() {
        positionBuffer.Release();
        positionBuffer = null;
    }

    void Update() {
        UpdateFunction();

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

        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));

        Graphics.DrawMeshInstancedProcedural(
            mesh, 
            0, 
            material, 
            bounds,
            resolution * resolution
        );
    }

    private void UpdateFunction() {
        elapsedTime += Time.deltaTime;

        if (!isTransitioning && elapsedTime >= functionDuration) {
            elapsedTime -= functionDuration;
            previousFunction = currentFunction;
            currentFunction = FunctionLibrary.NextFunctionName(currentFunction);
        } else if (isTransitioning && elapsedTime >= transitionDuration) {
            elapsedTime -= transitionDuration;
            previousFunction = null;
        }

        float time = Time.time;

        FunctionLibrary.Function f;

        if (previousFunction is FunctionLibrary.FunctionName prevFunc) {
            f = FunctionLibrary.Morph(prevFunc, currentFunction, elapsedTime / transitionDuration);
        } else {
            f = FunctionLibrary.GetFunction(currentFunction);
        }
    }
}
