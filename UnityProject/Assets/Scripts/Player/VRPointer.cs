using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Hand))]
public class VRPointer : MonoBehaviour
{
    [SerializeField] private float handVibrationStrength = 3000;
    public PointerEventData pointerEventData;
    private GameObject prevHitObject;
    private GameObject pressedObject;

    [HideInInspector]
    public Hand controller;
    [HideInInspector]
    public VRLaserPointer laserPointer;

    private void Start()
    {
        controller = GetComponent<Hand>();
        laserPointer = GetComponent<VRLaserPointer>();
    }

    public void Process()
    {
        if (pointerEventData.pointerCurrentRaycast.distance > 0)
        {
            laserPointer.active = true;
            laserPointer.SetDistance(pointerEventData.pointerCurrentRaycast.distance + .01f);
        }
        else
        {
            laserPointer.active = false;
        }

        GameObject hitObject = pointerEventData.pointerCurrentRaycast.gameObject;
        if (hitObject != null && prevHitObject != hitObject)
        {
            controller.controller.TriggerHapticPulse((ushort) (handVibrationStrength / 6f));
            PlaySound
        }
        if (controller.controller.GetHairTrigger())
        {
            pointerEventData.pressPosition = pointerEventData.position;
            pointerEventData.pointerPressRaycast = pointerEventData.pointerCurrentRaycast;
            if (hitObject)
            {
                ExecuteEvents.ExecuteHierarchy(hitObject, pointerEventData, ExecuteEvents.pointerDownHandler);

                if (pressedObject != hitObject)
                {
                    controller.controller.TriggerHapticPulse((ushort) handVibrationStrength);
                    ExecuteEvents.ExecuteHierarchy(hitObject, pointerEventData, ExecuteEvents.pointerClickHandler);
                    pressedObject = hitObject;
                }
            }
        }
        else
        {
            ReleaseObject();
        }

        prevHitObject = hitObject;
    }

    private void ReleaseObject()
    {
        if (pressedObject)
        {
            ExecuteEvents.Execute(pressedObject, pointerEventData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(pressedObject, pointerEventData, ExecuteEvents.endDragHandler);
        }
        pressedObject = null;
    }
}
