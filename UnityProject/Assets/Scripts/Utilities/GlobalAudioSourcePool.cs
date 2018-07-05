using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalAudioSourcePool : MonoBehaviour {
    [SerializeField] private AudioSource[] audioSources;
    private Queue<AudioSource> audioSourceQueue;

    private void Start()
    {
        audioSourceQueue = new Queue<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            audioSourceQueue.Enqueue(audioSource);
        }
    }

    public AudioSource GetAudioSource()
    {
        AudioSource nextSource = audioSourceQueue.Dequeue();
        audioSourceQueue.Enqueue(nextSource);
        return nextSource;
    }
}
