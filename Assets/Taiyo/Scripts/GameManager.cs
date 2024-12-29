using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public bool isGameOver;
    public bool isGameStart;


    [SerializeField] CinemachineVirtualCamera vCamera;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] float orthoSizeConstruct;
    [SerializeField] float orthoSizeNormal;
    [SerializeField] GameObject panelMessage;
    [SerializeField] GameObject textMessagePfb;
    [SerializeField] public GameObject hPbarPartPfb;
    [SerializeField] public GameObject hpbarFolder;


    [SerializeField] public GameObject equipEffectPfb;
    [SerializeField] public GameObject damageEffectPfb;
    [SerializeField] public GameObject peelOffEffectPfb;
    [SerializeField] public GameObject effectFolder;

    [SerializeField] EnemySpawner enemySpawner;

    GameObject textMessageOnThisFrame;


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

        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void LateUpdate()
    {
        if (textMessageOnThisFrame) textMessageOnThisFrame = null;
    }

    public void StartConstructMode()
    {
        vCamera.m_Lens.OrthographicSize = orthoSizeConstruct;
        ConstructManager.instance.StartConstructMode();
        player.StartConstructMode();
        SetTimeScale(0);
    }

    public void EndConstructMode()
    {
        if (!player.CanEndConstructMode())
        {
            return;
        }


        vCamera.m_Lens.OrthographicSize = orthoSizeNormal;
        ConstructManager.instance.EndConstructMode();
        player.EndConstructMode();
        SetTimeScale(1);

        if (!isGameStart)
        {
            isGameStart = true;
            PartSpawner.instance.StartWave();
            enemySpawner.StartWave();
        }
    }

    public void DisplayMessage(string message)
    {
        GameObject textMessage = Instantiate(textMessagePfb, panelMessage.transform);
        textMessage.GetComponent<TMPro.TextMeshProUGUI>().text = message;

        //同じフレームで複数のメッセージが表示される場合、重ならないようにする
        if (textMessageOnThisFrame != null)
        {
            textMessage.transform.localPosition = textMessageOnThisFrame.transform.localPosition + new Vector3(0, 100, 0);
        }
        textMessageOnThisFrame = textMessage;
    }

    public void GameOver()
    {
        isGameOver = true;
        DisplayMessage("Game Over");
    }

    public void ShakeCamera(float strength)
    {
        //impulseSource.GenerateImpulse();
        impulseSource.GenerateImpulseWithForce(strength);
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

}

