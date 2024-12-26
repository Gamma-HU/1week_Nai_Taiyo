using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectPoint : MonoBehaviour
{
    [SerializeField] public bool isFrameMode;
    [SerializeField] public List<float> angleList = new List<float>();

    public bool isConected;
    public ConnectPoint targetConnectPoint;
    public bool isParent;

    public void Connect(ConnectPoint targetPoint)
    {
        isConected = true;
        targetConnectPoint = targetPoint;

        targetConnectPoint.isConected = true;
        targetConnectPoint.targetConnectPoint = this;
        targetConnectPoint.isParent = true;
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
    }

    public void QuitConnect()
    {
        isConected = false;
        targetConnectPoint.isConected = false;
        targetConnectPoint.isParent = false;
        targetConnectPoint.targetConnectPoint = null;
        targetConnectPoint = null;

        isParent = false;
    }


}
