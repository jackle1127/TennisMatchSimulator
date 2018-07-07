using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleportController : MonoBehaviour {
    public Transform playerCamera;
    public SteamVR_Fade cameraFader;
    public float teleportFadeDuration = .5f;
    public Color teleportFadeColor = Color.white;

    private bool teleporting = false;
    private Color teleportClearColor = new Color(0, 0, 0, 0);

    public void TeleportTo(Matrix4x4 destinationMatrix)
    {
        StartCoroutine(TeleportCoroutine(destinationMatrix));
    }

    IEnumerator TeleportCoroutine(Matrix4x4 destinationMatrix)
    {
        if (cameraFader)
        {
            cameraFader.OnStartFade(teleportFadeColor, teleportFadeDuration, true);
            yield return new WaitForSecondsRealtime(teleportFadeDuration);
            cameraFader.OnStartFade(teleportClearColor, teleportFadeDuration, true);
        }

        TennisSim.Utility.ApplyMatrixToTransformSpecific(transform, destinationMatrix, true, true, false);
        /*transform.rotation = Quaternion.Euler(0,
            Utility.VectorAngleOnPlane(transform.forward, playerCamera.forward, Vector3.up), 0)
            * transform.rotation;*/
        Vector3 delta = playerCamera.position - transform.position;
        delta.y = 0;
        transform.position -= delta;
    }
}
