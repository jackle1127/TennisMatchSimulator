using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControlUIController : MonoBehaviour {
    [SerializeField] private RectTransform controlUIContainer;

	// Use this for initialization
	void Start () {
		if (controlUIContainer)
        {
            controlUIContainer.parent = transform;
            controlUIContainer.localRotation = Quaternion.identity;
            controlUIContainer.localPosition = Vector3.zero;
            controlUIContainer.localScale = Vector3.one;
            controlUIContainer.offsetMin = controlUIContainer.offsetMax = Vector2.zero;
        }
    }
}
