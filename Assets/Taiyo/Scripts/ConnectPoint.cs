using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectPoint : MonoBehaviour
{
    [SerializeField] public bool isFrameMode;

    public bool isConected;
    public ConnectPoint targetConnectPoint;
    public bool isParent;

    Color colorNormal = new Color(1, 1, 1, 1);
    Color colorConnected = new Color(0, 1, 0, 1);


    public List<float> angleList = new List<float>();

    void Awake()
    {
        angleList.Clear();
        for (int i = 0; i < 8; i++)
        {
            angleList.Add(i * 45);
        }
    }

    public void Connect(ConnectPoint targetPoint)
    {
        isConected = true;
        targetConnectPoint = targetPoint;

        targetConnectPoint.isConected = true;
        targetConnectPoint.targetConnectPoint = this;
        targetConnectPoint.isParent = true;

        SetColor(colorConnected);
    }

    public void Connect(ConnectPoint targetPoint, bool isParent)
    {
        isConected = true;
        targetConnectPoint = targetPoint;


        targetConnectPoint.isConected = true;
        targetConnectPoint.targetConnectPoint = this;
        if (isParent)
        {
            this.isParent = false;
        }
        else
        {
            targetConnectPoint.isParent = true;
        }

        SetColor(colorConnected);
    }

    public void QuitConnect()
    {
        isConected = false;
        targetConnectPoint.isConected = false;
        targetConnectPoint.isParent = false;
        targetConnectPoint.targetConnectPoint = null;
        targetConnectPoint = null;

        isParent = false;

        SetColor(colorNormal);
    }

    public void ActivateImage(bool isActive)
    {
        GetComponent<Image>().enabled = isActive;
    }

    public void SetColor(Color color)
    {

        GetComponent<Image>().color = color;

    }


}
