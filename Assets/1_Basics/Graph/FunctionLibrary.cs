using UnityEngine;

using static UnityEngine.Mathf;

public static class FunctionLibrary {
    public delegate Vector3 Function(float u, float v, float t);

    public enum FunctionName { Wave, MultiWave, Ripple, Sphere, Torus }

    static Function[] functions = { Wave, MultiWave, Ripple, Sphere, Torus };

    static readonly int functionCount = System.Enum.GetNames(typeof(FunctionName)).Length;

    public static Function GetFunction(FunctionName functionName) {
        return functions[(int)functionName];
    }

    public static Function Morph(FunctionName a, FunctionName b, float progress) {
        Function funcA = GetFunction(a);
        Function funcB = GetFunction(b);
        float adjustedProgress = SmoothStep(0, 1, progress); 

        return (float u, float v, float t) => {
            Vector3 resultA = funcA(u, v, t);
            Vector3 resultB = funcB(u, v, t);
            return Vector3.Lerp(resultA, resultB, adjustedProgress);
        };
    }

    public static FunctionName NextFunctionName(FunctionName name) {
        int number = ((int)name + 1) % functionCount;
        return (FunctionName)number;
    }

    public static Vector3 Wave(float u, float v, float t) {
        float y = Sin(PI * (u + v + t));
        return new(u, y, v);
    }

    public static Vector3 MultiWave(float u, float v, float t) {
        float y = Sin(PI * (u + t));
        y += Sin(2 * PI * (v + t)) / 2;
        y += Sin(PI * (u + v + 0.25f * t));
        y /= 2.5f;
        return new(u, y, v);
    }

    public static Vector3 Ripple(float u, float v, float t) {
        Vector2 vector = new(u, v);
        float d = vector.magnitude;
        float y = Sin(PI * (4 * d - t)) / (1 + 10 * d);
        return new(u, y, v);
    }

    public static Vector3 Sphere(float u, float v, float t) {
        float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4 * v + t));
        float s = r * Cos(0.5f * PI * v);

        return new(
            s * Sin(PI * u),
            r * Sin(v * PI * 0.5f),
            s * Cos(PI * u)
        );
    }

    public static Vector3 Torus(float u, float v, float t) {
        float r1 = 0.7f + 0.1f * Sin(PI * (6f * u + 0.5f * t));
		float r2 = 0.15f + 0.05f * Sin(PI * (8f * u + 4f * v + 2f * t));
        float s = r1 + r2 * Cos(PI * v);

        return new(
            s * Sin(PI * u),
            r2 * Sin(PI * v),
            s * Cos(PI * u)
        );
    }
}
