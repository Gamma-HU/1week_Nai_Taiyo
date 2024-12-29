using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Start is called before the first frame update
    public void Gamestart()
    {
        DOTween.KillAll();
        SceneManager.LoadScene("SceneTitle_Taiyo");
    }
}
