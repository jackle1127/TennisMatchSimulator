using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola {
    public List<Vector3> nodes = new List<Vector3>();
    public GameObject hitObject;
    public Vector3 destination;
    public Vector3 normal;

    public Vector3 GetNormalizedPathLocation(float alpha) {
        if (nodes.Count == 0) return new Vector3(-999, -999, -999);
        float actualAlpha = alpha * nodes.Count;
        int index1 = (int)(actualAlpha);
        int index2 = index1 + 1;
        if (index2 >= nodes.Count) return nodes[nodes.Count - 1];

        return Vector3.Lerp(nodes[index1], nodes[index2], actualAlpha - index1);
    }

    public Matrix4x4 GetNormalizedPathMatrix(float alpha) {
        if (nodes.Count == 0) return Matrix4x4.zero;
        float actualAlpha = alpha * (nodes.Count - 1);
        int index1 = (int)(actualAlpha);
        int index2 = index1 + 1;
        Vector3 fromPoint, toPoint;
        if (index2 >= nodes.Count) {
            fromPoint = nodes[index1];
            toPoint = fromPoint + (fromPoint - nodes[index1 - 1]);
        } else {
            fromPoint = Vector3.Lerp(nodes[index1], nodes[index2], actualAlpha - index1);
            toPoint = nodes[index2];
        }

        return Matrix4x4.LookAt(fromPoint, toPoint, Vector3.up);
    }

    public static Parabola Cast(Vector3 origin, Vector3 velocity, float stepSize, int maxSteps, int layerMask) {
        Parabola result = new Parabola();
        result.nodes.Add(origin);
        Vector3 currentLocation = origin;
        Vector3 prevLocation = currentLocation;
        RaycastHit hit;
        result.destination = new Vector3(-999, -999, -999);
        for (int i = 1; i <= maxSteps; i++) {
            currentLocation += velocity * stepSize;
            velocity.y -= 9.8f * stepSize;
            Debug.DrawRay(prevLocation, currentLocation - prevLocation);
            if (Physics.Raycast(prevLocation, currentLocation - prevLocation, out hit,
                    (currentLocation - prevLocation).magnitude, layerMask)) {
                result.nodes.Add(hit.point);
                result.normal = hit.normal;
                result.hitObject = hit.collider.gameObject;
                result.destination = hit.point;
                break;
            }
            prevLocation = currentLocation;
            result.nodes.Add(currentLocation);
        }
        return result;
    }

    public static Parabola Cast(Vector3 origin, Vector3 velocity, float stepSize, int steps) {
        return Cast(origin, velocity, stepSize, steps, Physics.DefaultRaycastLayers);
    }

    public bool hasDestination
    {
        get { return destination.x != -999; }
    }
}
