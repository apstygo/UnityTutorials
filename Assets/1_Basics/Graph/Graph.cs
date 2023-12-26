using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Graph: MonoBehaviour {
    [SerializeField]
    Transform pointPrefab;

    [SerializeField, Range(10, 200)]
    int resolution;

    [SerializeField, Min(0f)]
    float functionDuration = 1f;

    [SerializeField, Min(0f)]
    float transitionDuration = 0.5f;

    private FunctionLibrary.FunctionName? previousFunction = null;

    private FunctionLibrary.FunctionName currentFunction = FunctionLibrary.FunctionName.Wave;

    private Transform[] points;
    private float elapsedTime;

    private Vector3 scale {
        get { return Vector3.one * 2f / resolution; }
    }

    private bool isTransitioning {
        get { return previousFunction.HasValue; }
    }

    void Awake() {
        points = new Transform[resolution * resolution];

        for (int i = 0; i < resolution; i++) {
            for (int j = 0; j < resolution; j++) {
                Transform point = Instantiate(pointPrefab);
                points[i * resolution + j] = point;
                point.SetParent(transform, false);
            }
        }
    }

    void Update() {
        UpdateFunction();
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

        for (int k = 0; k < points.Length; k++) {
            int i = k / resolution;
            int j = k % resolution;
            float u = j * 2f / resolution - 1f + scale.x / 2f;
            float v = i * 2f / resolution - 1f + scale.z / 2f;
            points[k].localPosition = f(u, v, time);
            points[k].localScale = scale;
        }
    }
}
