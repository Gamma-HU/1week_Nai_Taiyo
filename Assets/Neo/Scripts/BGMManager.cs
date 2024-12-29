using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    [SerializeField] AudioSource bgmAudioSource;

    public static float bgmVolume = 0.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //スライダーでBGMの音量調整
    public void SetBGMVolume(float volume)
    {
        bgmAudioSource.volume = volume;
        bgmVolume = volume;
    }
}
