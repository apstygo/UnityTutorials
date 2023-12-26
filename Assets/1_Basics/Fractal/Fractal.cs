using UnityEngine;

public class Fractal: MonoBehaviour {
    struct FractalPart {
        public Vector3 direction;
        public Vector3 worldPosition;
        public Quaternion rotation;
        public Quaternion worldRotation;
    }

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;

    [SerializeField, Range(1, 8)]
    int depth = 4;

    private static readonly Vector3[] directions = { 
        Vector3.up, 
        Vector3.right, 
        Vector3.left, 
        Vector3.forward, 
        Vector3.back
    };

    private static readonly Quaternion[] rotations = {
        Quaternion.identity,
        Quaternion.Euler(0, 0, -90),
        Quaternion.Euler(0, 0, 90),
        Quaternion.Euler(90, 0, 0),
        Quaternion.Euler(-90, 0, 0)
    };

    private FractalPart[][] parts;
    private Matrix4x4[][] matrices;

    void Awake() {
        parts = new FractalPart[depth][];
        matrices = new Matrix4x4[depth][];

        for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) {
            parts[i] = new FractalPart[length];
            matrices[i] = new Matrix4x4[length];
        }

        parts[0][0] = CreatePart(0);

        for (int levelIndex = 1; levelIndex < parts.Length; levelIndex++) {
            FractalPart[] levelParts = parts[levelIndex];

            for (int partIndex = 0; partIndex < levelParts.Length; partIndex += 5) {
                for (int childIndex = 0; childIndex < 5; childIndex++) {
                    levelParts[partIndex + childIndex] = CreatePart(childIndex);
                }
            }
        }
    }

    void Update() {
        var deltaRotation = Quaternion.Euler(0, 22.5f * Time.deltaTime, 0);

        var rootPart = parts[0][0];
        rootPart.rotation *= deltaRotation;
        rootPart.worldRotation = rootPart.rotation;
        parts[0][0] = rootPart;
        matrices[0][0] = Matrix4x4.TRS(rootPart.worldPosition, rootPart.worldRotation, Vector3.one);

        float scale = 1;

        for (int levelIndex = 1; levelIndex < parts.Length; levelIndex++) {
            scale *= 0.5f;

            FractalPart[] parentParts = parts[levelIndex - 1];
            FractalPart[] levelParts = parts[levelIndex];

            for (int partIndex = 0; partIndex < levelParts.Length; partIndex++) {
                FractalPart parent = parentParts[partIndex / 5];
                FractalPart part = levelParts[partIndex];
                part.rotation *= deltaRotation;

                part.worldRotation = parent.worldRotation * part.rotation;

                part.worldPosition = parent.worldPosition +
                    parent.worldRotation * (1.5f * scale * part.direction);

                levelParts[partIndex] = part;

                matrices[levelIndex][partIndex] = Matrix4x4.TRS(
                    part.worldPosition, 
                    part.worldRotation,
                    Vector3.one * scale
                );
            }
        }
    }

    private FractalPart CreatePart(int childIndex) => new() {
        direction = directions[childIndex],
        rotation = rotations[childIndex]
    };
}
