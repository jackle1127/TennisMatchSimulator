using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(Hand))]
public class HandTeleportController : MonoBehaviour
{
    public ParabolaCaster caster;
    public Transform casterDirection;
    public PlayerTeleportController playerTeleportController;
    public TeleportAudioController teleportAudio;
    public ushort hapticStrong = 1000, hapticLoop = 500;
    private Hand steamVRHand;
    private Matrix4x4 currentDestination;
    private bool canceled;
    private bool prevActive;

    // Use this for initialization
    void Start()
    {
        steamVRHand = GetComponent<Hand>();
    }

    // Update is called once per frame
    void Update()
    {
        if (steamVRHand.controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            canceled = true;
        }
        Vector2 inputDirection = steamVRHand.controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0);
        bool isActive = false;
        if (inputDirection.magnitude > .8f)
        {
            if (!canceled)
            {
                currentDestination = caster.Cast(casterDirection, inputDirection);

                if (caster.IsCastingFrom(casterDirection))
                {
                    isActive = true;
                }
            }
            else
            {
                teleportAudio.Cancel();
                caster.Hide(casterDirection);
            }
        }
        else
        {
            if (caster.IsCastingFrom(casterDirection) && currentDestination != Matrix4x4.zero && !canceled)
            {
                teleportAudio.Go();
                playerTeleportController.TeleportTo(currentDestination);
                currentDestination = Matrix4x4.zero;
            }
            else if (caster.IsCastingFrom(casterDirection))
            {
                teleportAudio.Cancel();
            }
            caster.Hide(casterDirection);
            canceled = false;
        }
        if (!prevActive && isActive)
        {
            teleportAudio.StartTeleporting();
        }
        if (prevActive != isActive)
        {
            steamVRHand.controller.TriggerHapticPulse(hapticStrong);
        }
        if (isActive)
        {
            //SteamVR_Controller.Input((int)steamVRHand.controller.index).TriggerHapticPulse(100);
            steamVRHand.controller.TriggerHapticPulse(hapticLoop);
        }

        prevActive = isActive;
    }
}
