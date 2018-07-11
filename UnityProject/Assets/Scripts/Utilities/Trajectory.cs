using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trajectory {
    public float scale = 0;
    public float translateX = 0;
    public float translateY = 0;

    public Trajectory() { }

    public Trajectory(Vector2 pointA, Vector2 pointB, Vector2 pointC) : this()
    {
        Calculate(pointA, pointB, pointC);
    }

    public Trajectory(Trajectory trajectoryToCopy) : this()
    {
        Copy(trajectoryToCopy);
    }

    public void Copy(Trajectory trajectoryToCopy)
    {
        scale = trajectoryToCopy.scale;
        translateX = trajectoryToCopy.translateX;
        translateY = trajectoryToCopy.translateY;
    }

    public void Calculate(Vector2 pointA, Vector2 pointB, Vector2 pointC)
    {
        scale = (pointB.y - pointC.y - (pointB.x - pointC.x) * (pointA.y - pointB.y) / (pointA.x - pointB.x)) / (pointB.x * pointB.x - pointC.x * pointC.x - (pointA.x + pointB.x) * (pointB.x - pointC.x));
        translateX = -((pointA.y - pointB.y) / (scale * (pointA.x - pointB.x)) - pointA.x - pointB.x) / 2f;
        float temp = pointA.x - translateX;
        translateY = pointA.y - scale * temp * temp;
    }

    public float Evaluate(float x)
    {
        float temp = x - translateX;
        return scale * temp * temp + translateY;
    }

    public float[] SolveForX(float y)
    {
        if ((y - translateY) / scale < 0) return null;
        float temp = Mathf.Sqrt((y - translateY) / scale);
        return new float[]{-temp + translateX, temp + translateX};
    }
}
