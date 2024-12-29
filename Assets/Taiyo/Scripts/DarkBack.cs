using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DarkBack : MonoBehaviour
{
    float distance;
    float distanceMax = 1000f;

    void Update()
    {
        distance = (GameManager.instance.player.transform.position - new Vector3(0, 0, 0)).magnitude;
        GetComponent<Image>().color = new Color(0, 0, 0, 1 - distance / distanceMax);
    }
}
