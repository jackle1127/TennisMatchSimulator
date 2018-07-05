using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioController : MonoBehaviour {
    public void PlayAudio(AudioClip clip)
    {
        AudioUtility.PlayGlobalAudio(clip);
    }
}
