using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSetting : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource;
    [SerializeField] AudioSource seAudioSource;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider seSlider;

    AudioSetting instance;
    float bgmVolume = 0.5f;
    float seVolume = 0.5f;

    private void Awake()
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

    void Start()
    {
        bgmAudioSource = GameObject.Find("BGMAudioSource").GetComponent<AudioSource>();
        seAudioSource = GameObject.Find("SEAudioSource").GetComponent<AudioSource>();
        bgmSlider = GameObject.Find("BGMSlider").GetComponent<Slider>();
        seSlider = GameObject.Find("SESlider").GetComponent<Slider>();

        bgmSlider.value = bgmVolume;
        seSlider.value = seVolume;
        bgmAudioSource.volume = bgmVolume;
        seAudioSource.volume = seVolume;


        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmAudioSource.volume = volume;
    }

    public void SetSEVolume(float volume)
    {
        seVolume = volume;
        seAudioSource.volume = volume;
    }

}
