#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
	StructuredBuffer<uint> _Hashes;
#endif

float4 _Config;

void ConfigureProcedural () {
	#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        float resolution = _Config.x;
        float step = _Config.y;
        float verticalOffset = _Config.z;

        float v = floor(step * unity_InstanceID + 0.00001);
        float u = unity_InstanceID - resolution * v;

        unity_ObjectToWorld = 0.0;

        unity_ObjectToWorld._m03_m13_m23_m33 = float4(
            step * (u + 0.5) - 0.5,
            verticalOffset * ((1.0 / 255.0) * (_Hashes[unity_InstanceID] >> 24) - 0.5),
            step * (v + 0.5) - 0.5,
            1.0
        );

        unity_ObjectToWorld._m00_m11_m22 = step;
	#endif
}

float3 GetHashColor() {
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        uint hash = _Hashes[unity_InstanceID];

        return (1.0 / 255.0) * float3(
            hash & 255,
            (hash >> 8) & 255,
            (hash >> 16) & 255
        );
    #else
        return 1.0;
    #endif
}
