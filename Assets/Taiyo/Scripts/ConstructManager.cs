using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConstructManager : MonoBehaviour
{
    public static ConstructManager instance;
    Player player;


    [SerializeField] float constructRange;
    [SerializeField] float connectPointDistance;
    public float ConnectPointDistance => connectPointDistance;

    List<Part> partsConstructingList = new List<Part>();
    public List<Part> PartsConstructingList => partsConstructingList;

    bool isConstructMode;
    Part partPicking;

    [SerializeField] GameObject fuelVolumeText;

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
    }

    void Start()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!isConstructMode)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            PickPart();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (partPicking != null)
            {
                if (partPicking.CanConnect)
                {
                    partPicking.Connect();
                    partPicking = null;
                }
                else
                {
                    partPicking.QuitPick();
                    partPicking = null;
                }
            }
        }


    }

    public void StartConstructMode()
    {
        foreach (Part part in PartSpawner.instance.floatingPartList)
        {
            if ((part.transform.position - player.transform.position).magnitude < constructRange)
            {
                SetConstructingParts(part);
            }
        }

        isConstructMode = true;
    }

    public void EndConstructMode()
    {
        foreach (Part part in partsConstructingList)
        {
            part.EndConstructMode();
        }
        partsConstructingList.Clear();

        isConstructMode = false;
    }

    public void SetConstructingParts(Part part)
    {
        partsConstructingList.Add(part);
        part.StartConstructMode();
    }

    void PickPart()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -10;

        // レイを飛ばす
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // レイキャストがパーツに当たった場合
        if (hit.collider != null && hit.collider.gameObject.GetComponent<Part>())
        {
            Part part = hit.collider.gameObject.GetComponent<Part>();
            if (part.isConstructMode)
            {
                if (part.isCockpit)
                {
                    return;
                }
                part.Pick();
                partPicking = part;
            }
        }
    }
}
