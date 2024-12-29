using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private AudioSource buttoneffectsound;
    public static int hatchcount;
    void Start()
    {
        //タイトルの時点で最大のフレームレートを60で固定しておきます。シーン遷移をしても適用され続けます。
        Application.targetFrameRate = 60;
    }

    public void StartButtonPressed()
    {
        buttoneffectsound.Play();
        //シーン遷移
        //Swinging.SwingingTween.Kill();
        DOTween.KillAll();
        hatchcount = 1;
        SceneManager.LoadScene("Main");
        Debug.Log("シーン遷移！！！");
    }
}
