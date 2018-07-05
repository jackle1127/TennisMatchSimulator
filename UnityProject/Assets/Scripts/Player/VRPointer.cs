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
    private GameObject draggedObject;

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
            controller.controller.TriggerHapticPulse((ushort)(handVibrationStrength / 6f));
        }
        if (controller.controller.GetHairTriggerDown())
        {
            pointerEventData.button = PointerEventData.InputButton.Left;
            pointerEventData.pressPosition = pointerEventData.position;
            pointerEventData.pointerPressRaycast = pointerEventData.pointerCurrentRaycast;
            if (hitObject)
            {
                GameObject currentPressed = ExecuteEvents.ExecuteHierarchy(hitObject, pointerEventData, ExecuteEvents.pointerDownHandler);
                pointerEventData.pointerPress = currentPressed;

                //Debug.Log("DOWN: " + currentPressed);
                pointerEventData.pointerDrag = currentPressed;
                pointerEventData.dragging = true;
                controller.controller.TriggerHapticPulse((ushort)handVibrationStrength);
                ExecuteEvents.ExecuteHierarchy(hitObject, pointerEventData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(currentPressed, pointerEventData, ExecuteEvents.beginDragHandler);
                pressedObject = currentPressed;
                draggedObject = pressedObject;
            }
        }
        if (controller.controller.GetHairTriggerUp())
        {
            ReleaseObject();
        }
        if (draggedObject != null)
        {
            ExecuteEvents.Execute(draggedObject, pointerEventData, ExecuteEvents.dragHandler);
        }
        prevHitObject = hitObject;
    }

    private void ReleaseObject()
    {
        if (pressedObject)
        {
            ExecuteEvents.Execute(pressedObject, pointerEventData, ExecuteEvents.pointerUpHandler);
            pressedObject = null;
        }
        if (draggedObject != null)
        {
            ExecuteEvents.Execute(draggedObject, pointerEventData, ExecuteEvents.endDragHandler);
            draggedObject = null;
        }
        pointerEventData.dragging = false;
        pointerEventData.pointerPress = null;
    }
}
