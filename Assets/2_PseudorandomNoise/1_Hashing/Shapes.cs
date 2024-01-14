using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public static class Shapes {
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
    public struct Job: IJobFor {
        [WriteOnly]
        public NativeArray<float3x4> positions;

        [WriteOnly]
        public NativeArray<float3x4> normals;

        public float resolution;
        public float invResolution;
        public float3x4 positionTRS;

        float4x3 TransformVectors(float3x4 trs, float4x3 p, float w = 1f) => float4x3(
            trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x * w,
            trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y * w,
            trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z * w
        );

        public void Execute(int i) {
            float4 i4 = i * 4.0f + float4(0.0f, 1.0f, 2.0f, 3.0f);
            float4x2 uv;
            uv.c1 = floor(invResolution * i4 + 0.00001f);
			uv.c0 = invResolution * (i4 - resolution * uv.c1 + 0.5f) - 0.5f;
			uv.c1 = invResolution * (uv.c1 + 0.5f) - 0.5f;

            positions[i] = transpose(TransformVectors(positionTRS, float4x3(uv.c0, 0, uv.c1)));

            float3x4 n = transpose(TransformVectors(positionTRS, float4x3(0, 1, 0), 0));
            normals[i] = float3x4(normalize(n.c0), normalize(n.c1), normalize(n.c2), normalize(n.c3));
        }

        public static JobHandle ScheduleParallel(
            NativeArray<float3x4> positions,
            NativeArray<float3x4> normals,
            int resolution,
            float4x4 trs,
            JobHandle dependency
        ) {
            return new Job {
                positions = positions,
                normals = normals,
                resolution = resolution,
                invResolution = 1f / resolution,
                positionTRS = float3x4(trs.c0.xyz, trs.c1.xyz, trs.c2.xyz, trs.c3.xyz)
            }
            .ScheduleParallel(positions.Length, resolution, dependency);
        }
    }
}