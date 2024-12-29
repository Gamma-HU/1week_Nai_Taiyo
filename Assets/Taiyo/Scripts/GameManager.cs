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


    [SerializeField] CinemachineVirtualCamera vCamera;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] float orthoSizeConstruct;
    [SerializeField] float orthoSizeNormal;
    [SerializeField] GameObject panelMessage;
    [SerializeField] GameObject textMessagePfb;
    [SerializeField] public GameObject hPbarPartPfb;
    [SerializeField] public GameObject hpbarFolder;

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
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = strength;
        impulseSource.m_ImpulseDefinition.m_FrequencyGain = strength;
        impulseSource.GenerateImpulse();
    }

}

