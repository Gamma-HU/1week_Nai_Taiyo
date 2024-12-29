using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    public static SEManager instance;

    [SerializeField] AudioSource seAudioSource;

    public static float seVolume = 0.5f;

    /*
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    */

    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    //スライダーでSEの音量調整
    public void SetSEVolume(float volume)
    {
        seAudioSource.volume = volume;
        /*パブリックなseVolumeに数値を入れておいて、他のシーンの始めに
        このseVolumeから音量を参照したい。*/
        seVolume = volume;
    }
}
