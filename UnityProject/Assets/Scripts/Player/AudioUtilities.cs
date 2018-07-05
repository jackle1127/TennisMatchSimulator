using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUtility  {
    public static void PlayGlobalAudio(AudioClip clip)
    {
        GlobalAudioSourcePool audioController = GameObject.FindObjectOfType<GlobalAudioSourcePool>();
        if (audioController)
        {
            AudioSource audioSource = audioController.GetAudioSource();
            if (audioSource)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}
