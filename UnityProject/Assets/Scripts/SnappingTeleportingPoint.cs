using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnappingTeleportingPoint : MonoBehaviour
{
    public Transform snappingPoint;
    public GameObject highlightObject;
    public GameObject[] objectsToHideWhenHighlight;

    public void Highlight()
    {
        if (highlightObject)
        {
            highlightObject.SetActive(true);
        }
        foreach (GameObject objectToTurnOff in objectsToHideWhenHighlight)
        {
            objectToTurnOff.SetActive(false);

        }
    }

    public void Unhighlight()
    {
        if (highlightObject)
        {
            highlightObject.SetActive(false);
        }
        foreach (GameObject objectToTurnOff in objectsToHideWhenHighlight)
        {
            objectToTurnOff.SetActive(true);

        }
    }
}
