using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VolumeSlider : MonoBehaviour //ボリューム設定のスライダー
{
	[SerializeField] TextMeshProUGUI BGMValue;
	[SerializeField] TextMeshProUGUI SEValue;
    public void SetBGMVolume(float volume)
    {
        BGMManager.instance.SetBGMVolume(volume);
	    BGMValue.text = volume.ToString("f2");
    }
    public void SetSEVolume(float volume)
    {
        SEManager.instance.SetSEVolume(volume);
	    SEValue.text = volume.ToString("f2");
    }
}
