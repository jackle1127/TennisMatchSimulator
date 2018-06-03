using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempController : MonoBehaviour
{
    public ParabolaCaster caster;
    public PlayerTeleportController playerTeleportController;
    private Matrix4x4 currentDestination;
    private bool canceled;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            canceled = true;
        }
        Vector2 inputDirection = Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up;
        if (inputDirection.magnitude > .8f)
        {
            if (!canceled)
            {
                currentDestination = caster.Cast(transform, inputDirection);
            }
            else
            {
                caster.Hide(transform);
            }
        }
        else
        {
            if (caster.IsCastingFrom(transform) && currentDestination != Matrix4x4.zero && !canceled)
            {
                playerTeleportController.TeleportTo(currentDestination);
                currentDestination = Matrix4x4.zero;
            }
            caster.Hide(transform);
            canceled = false;
        }
    }
}
