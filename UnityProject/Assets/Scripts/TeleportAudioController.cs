using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAudioController : MonoBehaviour {
    public AudioSource teleportStart, teleportLoop, teleportGo;

    public void StartTeleporting()
    {
        teleportStart.Play();
        teleportLoop.Play();
    }

    public void Go()
    {
        teleportLoop.Stop();
        teleportGo.Play();
    }

    public void Cancel()
    {
        teleportLoop.Stop();
    }
}
