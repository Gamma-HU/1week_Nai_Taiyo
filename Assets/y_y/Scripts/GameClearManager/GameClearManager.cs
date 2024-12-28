using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private float goalDistance = 1000f;
    [SerializeField] private GameObject ClearUI;
    private float current_distance = 0f;
    private GameObject playerGameObj;

    void Start()
    {
        playerGameObj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        current_distance = Vector3.Distance(this.gameObject.GetComponent<RectTransform>().anchoredPosition, playerGameObj.GetComponent<RectTransform>().anchoredPosition);
        if (current_distance > goalDistance)
        {
            GameClear();
        }
    }

    private void GameClear()
    {
        Time.timeScale = 0f;
        ClearUI.SetActive(true);
    }

    public void PushRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
