using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPicker : MonoBehaviour {
    [SerializeField] private Transform[] points;

	public Vector3 PickRandom()
    {
        float[] weights = new float[points.Length];

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.value;
        }

        return PickPointByWeights(weights);
    }

    public Vector3 PickRandomWithMultiplier(params float[] multipliers)
    {
        if (multipliers.Length != points.Length) return Vector3.zero;

        float[] weights = new float[points.Length];

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = Random.value * multipliers[i];
        }

        return PickPointByWeights(weights);
    }

    public Vector3 PickPointByWeights(params float[] weights)
    {
        if (weights.Length != points.Length) return Vector3.zero;
        float weightSum = 0;
        foreach (float weight in weights)
        {
            weightSum += weight;
        }

        Vector3 result = Vector3.zero;
        for (int i = 0; i < weights.Length; i++)
        {
            result += weights[i] * points[i].position / weightSum;
        }

        return result;
    }
}
