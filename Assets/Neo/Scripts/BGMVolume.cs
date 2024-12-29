using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMVolume : MonoBehaviour
{
    AudioSource[] audioSources;


    private void Start()
    {
        audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.CompareTag("MainCamera"))
            {
                audioSource.volume = VolumeManager.BGMVolume;
            }
            else
            {
                audioSource.volume *= VolumeManager.SEVolume;
            }
            Debug.Log(audioSource.gameObject.name);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
