using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HPbarPart : MonoBehaviour
{
    Slider slider;
    Part part;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        if (part)
        {
            transform.position = part.transform.position + new Vector3(0, -0.5f, 0);
        }
    }

    public void SetPart(Part part)
    {
        this.part = part;
    }

    public void SetHP(float value)
    {
        slider.value = value;
        if (value <= 0.1f)
        {
            slider.fillRect.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }
        else if (value <= 0.3f)
        {
            slider.fillRect.GetComponent<Image>().color = new Color(1, 1, 0, 1);
        }
        else
        {
            slider.fillRect.GetComponent<Image>().color = new Color(0, 1, 0, 1);
        }
    }
}
