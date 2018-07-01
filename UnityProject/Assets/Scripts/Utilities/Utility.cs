﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{
    public static void ApplyMatrixToTransform(Transform t, Matrix4x4 m)
    {
        ApplyMatrixToTransformSpecific(t, m, true, true, true);
    }

    public static void ApplyMatrixToTransformSpecific(Transform t, Matrix4x4 m,
            bool position, bool rotation, bool scale)
    {
        if (position)
            t.position = GetTranslateFromMatrix(m);
        if (rotation)
            t.rotation = GetRotationFromMatrix(m);
        if (scale)
            t.localScale = GetScaleFromMatrix(m);
    }

    public static Vector3 GetTranslateFromMatrix(Matrix4x4 m)
    {
        return new Vector3(m.m03, m.m13, m.m23);
    }

    public static Vector3 GetXAxisFromMatrix(Matrix4x4 m)
    {
        return new Vector3(m.m00, m.m10, m.m20);
    }

    public static Vector3 GetYAxisFromMatrix(Matrix4x4 m)
    {
        return new Vector3(m.m01, m.m11, m.m21);
    }

    public static Vector3 GetZAxisFromMatrix(Matrix4x4 m)
    {
        return new Vector3(m.m02, m.m12, m.m22);
    }

    public static Quaternion GetRotationFromMatrix(Matrix4x4 m)
    {
        return Quaternion.LookRotation(GetZAxisFromMatrix(m), Vector3.up);
    }

    public static Vector3 GetScaleFromMatrix(Matrix4x4 m)
    {
        return new Vector3(GetXAxisFromMatrix(m).magnitude, GetYAxisFromMatrix(m).magnitude, GetZAxisFromMatrix(m).magnitude);
    }

    public static float VectorAngleOnPlane(Vector3 from, Vector3 to, Vector3 planeNormal)
    {
        from = ProjectVectorToPlay(from, planeNormal).normalized;
        to = ProjectVectorToPlay(to, planeNormal).normalized;
        planeNormal = planeNormal.normalized;
        return Mathf.Rad2Deg * Mathf.Atan2(
            Vector3.Dot(planeNormal, Vector3.Cross(to, from)),
            Vector3.Dot(to, from));
    }

    public static Vector3 ProjectVectorToPlay(Vector3 v, Vector3 planeNormal)
    {
        planeNormal = planeNormal.normalized;
        return v - Vector3.Dot(planeNormal, v) * planeNormal;
    }
}