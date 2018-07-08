using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumePointPickerController : MonoBehaviour
{
    [SerializeField] private Transform minCorner, maxCorner;

    public Vector3 GetRandomPointInVolume()
    {
        Vector3 min = minCorner.position;
        Vector3 max = maxCorner.position;
        float ranX = Random.value;
        float ranY = Random.value;
        float ranZ = Random.value;
        return new Vector3(
            (1 - ranX) * min.x + ranX * max.x,
            (1 - ranY) * min.y + ranY * max.y,
            (1 - ranZ) * min.z + ranZ * max.z
        );
    }

    /*<summary>
     * Pick a random point within a volume with a bias. The high the bias towards a dimension,
     * the more probable that the point received is near the max value of that dimension.
     * 
     * Actual calculation: 
     *    alpha_i = rand ^ (1 / bias)
     *    result_i = (1 - alpha_i) * min_i + alpha_i * max_i
     *    (i is a dimension)
     * </summary>
     */
    public Vector3 GetRandomPointInVolumeWithBias(float biasX, float biasY, float biasZ)
    {
        Vector3 min = minCorner.position;
        Vector3 max = maxCorner.position;
        float ranX = Mathf.Pow(Random.value, 1 / biasX);
        float ranY = Mathf.Pow(Random.value, 1 / biasY);
        float ranZ = Mathf.Pow(Random.value, 1 / biasZ);
        return new Vector3(
            (1 - ranX) * min.x + ranX * max.x,
            (1 - ranY) * min.y + ranY * max.y,
            (1 - ranZ) * min.z + ranZ * max.z
        );
    }
}
