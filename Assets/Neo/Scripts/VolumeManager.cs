using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public static GameObject instance;
    [SerializeField] public static float BGMVolume, SEVolume;
    void Start()
    {
        if(instance == null)
        {
            instance = GameObject.Find("VolumeManager");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void setVolumeManager()
    {
        BGMVolume = BGMManager.bgmVolume;
        SEVolume = SEManager.seVolume;
    }
}
