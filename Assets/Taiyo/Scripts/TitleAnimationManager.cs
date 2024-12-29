using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleAnimationManager : MonoBehaviour
{
    float lifeTime = 11;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
        {
            SceneManager.LoadScene("SceneGame_Taiyo");
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene("SceneGame_Taiyo");
        }
    }
}
