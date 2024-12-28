using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;


    [SerializeField] CinemachineVirtualCamera vCamera;
    [SerializeField] float orthoSizeConstruct;
    [SerializeField] float orthoSizeNormal;
    [SerializeField] GameObject panelMessage;
    [SerializeField] GameObject textMessagePfb;

    GameObject textMessagePrev;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        player = GameObject.Find("Player").GetComponent<Player>();
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

        if (textMessagePrev != null)
        {
            textMessage.transform.position = textMessagePrev.transform.position + new Vector3(0, textMessagePrev.GetComponent<RectTransform>().rect.height, 0);
        }
        textMessagePrev = textMessage;
    }

}

