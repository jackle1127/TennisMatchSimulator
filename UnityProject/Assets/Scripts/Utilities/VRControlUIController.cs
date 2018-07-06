using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRControlUIController : ControlUIController
{
    [SerializeField] private RectTransform controlUIContainer;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float distanceFromPlayer = 2;
    [SerializeField] private float pitchOffset = 0;

	// Use this for initialization
	protected override void Start () {
		if (controlUIContainer)
        {
            controlUIContainer.parent = transform;
            controlUIContainer.localRotation = Quaternion.identity;
            controlUIContainer.localPosition = Vector3.zero;
            controlUIContainer.localScale = Vector3.one;
            controlUIContainer.offsetMin = controlUIContainer.offsetMax = Vector2.zero;
        }
        Hide();
    }

    public override void Show()
    {
        Vector3 cameraForward = new Vector3(playerCamera.forward.x, 0, playerCamera.forward.z).normalized;
        cameraForward *= Mathf.Cos(pitchOffset * Mathf.Deg2Rad);
        cameraForward.y = Mathf.Sin(pitchOffset * Mathf.Deg2Rad);
        transform.position = playerCamera.position + cameraForward * distanceFromPlayer;
        transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position, Vector3.up);
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }
}
