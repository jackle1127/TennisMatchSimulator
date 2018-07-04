using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VRInputModule : BaseInputModule
{
    [SerializeField] private VRPointer[] pointers;

    private Transform uiCamera;
    private Vector2 cameraCenter;

    protected override void Start()
    {
        GameObject uiCameraObject = new GameObject("UICamera");
        Camera cameraComponent = uiCameraObject.AddComponent<Camera>();
        cameraComponent.enabled = false;
        cameraComponent.clearFlags = CameraClearFlags.Nothing;
        cameraComponent.fieldOfView = 5;
        cameraComponent.nearClipPlane = .01f;
        cameraCenter = new Vector2(cameraComponent.pixelWidth, cameraComponent.pixelHeight) * .5f;

        uiCamera = uiCameraObject.transform;
        foreach (Canvas canvas in Resources.FindObjectsOfTypeAll<Canvas>())
        {
            canvas.worldCamera = cameraComponent;
        }
    }

    private void SnapCameraToPointer(VRPointer pointer)
    {
        Transform laserCaster = pointer.laserPointer.reference;
        uiCamera.position = laserCaster.position;
        uiCamera.rotation = Quaternion.LookRotation(laserCaster.up, -laserCaster.forward);
    }

    public override void Process()
    {
        foreach (VRPointer pointer in pointers)
        {
            SnapCameraToPointer(pointer);
            if (pointer.pointerEventData == null)
            {
                pointer.pointerEventData = new PointerEventData(eventSystem);
            }

            pointer.pointerEventData.position = cameraCenter;
            eventSystem.RaycastAll(pointer.pointerEventData, m_RaycastResultCache);
            pointer.pointerEventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();

            HandlePointerExitAndEnter(pointer.pointerEventData, pointer.pointerEventData.pointerCurrentRaycast.gameObject);
            pointer.Process();
        }
    }
}
