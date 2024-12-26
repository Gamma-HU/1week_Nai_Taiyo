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

        vCamera.m_Lens.OrthographicSize = orthoSizeNormal;
        ConstructManager.instance.EndConstructMode();
        player.EndConstructMode();
    }

}

