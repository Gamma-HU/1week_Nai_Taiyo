using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    Part partParent;
    bool isCollide; //他のパーツと接触しているか
    public bool IsCollide => isCollide;


    // Start is called before the first frame update
    void Start()
    {
        partParent = transform.parent.GetComponent<Part>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCollide) GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0.3f);
        else GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.3f);
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Part>() && collision.gameObject != partParent.gameObject)
        {
            Part part = collision.gameObject.GetComponent<Part>();
            if (!part.isConnected || partParent is Part_Frame || part is Part_Frame) return;
            else isCollide = true; //フレーム同士orパーツ同士で接触している場合
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Part>() && collision.gameObject != partParent.gameObject)
        {
            Part part = collision.gameObject.GetComponent<Part>();
            if (!part.isConnected || partParent is Part_Frame || part is Part_Frame) return;
            else isCollide = false;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Part>() && collision.gameObject != partParent.gameObject)
        {
            Part part = collision.gameObject.GetComponent<Part>();
            if (!part.isConnected || partParent is Part_Frame || part is Part_Frame) return;
            else isCollide = true; //フレーム同士orパーツ同士で接触している場合
        }
    }
}
