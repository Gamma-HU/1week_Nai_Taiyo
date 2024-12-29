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
    [SerializeField] List<AudioClip> bgmClips;
    [SerializeField] List<AudioClip> seClips;

    GameObject panelSeting;

    public static AudioSetting instance;
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
        panelSeting = GameObject.Find("PanelSetting");



        bgmSlider.value = bgmVolume;
        seSlider.value = seVolume;
        bgmAudioSource.volume = bgmVolume;
        seAudioSource.volume = seVolume;

        panelSeting.SetActive(false);


        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);

        bgmAudioSource.clip = bgmClips[0];
        bgmAudioSource.Play();
    }

    public void InitialSetting()
    {
        bgmAudioSource = GameObject.Find("BGMAudioSource").GetComponent<AudioSource>();
        seAudioSource = GameObject.Find("SEAudioSource").GetComponent<AudioSource>();
        bgmSlider = GameObject.Find("BGMSlider").GetComponent<Slider>();
        seSlider = GameObject.Find("SESlider").GetComponent<Slider>();
        panelSeting = GameObject.Find("PanelSetting");



        bgmSlider.value = bgmVolume;
        seSlider.value = seVolume;
        bgmAudioSource.volume = bgmVolume;
        seAudioSource.volume = seVolume;

        panelSeting.SetActive(false);


        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        seSlider.onValueChanged.AddListener(SetSEVolume);

        bgmAudioSource.clip = bgmClips[0];
        bgmAudioSource.Play();
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

    public void PlayBGM(int index)
    {
        bgmAudioSource.clip = bgmClips[index];
        bgmAudioSource.Play();
    }

    public void PlaySE(int index)
    {
        seAudioSource.PlayOneShot(seClips[index]);
    }

}
