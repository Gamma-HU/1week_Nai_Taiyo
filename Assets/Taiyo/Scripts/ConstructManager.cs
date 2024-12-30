using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public bool isConstructMode;
    Part partPicking;
    [SerializeField] GameObject buttonConstruct;
    [SerializeField] GameObject panelConstructScreen;
    [SerializeField] TextMeshProUGUI massText;
    [SerializeField] GameObject massPosObj;
    [SerializeField] TextMeshProUGUI fuelVolumeText;
    [SerializeField] TextMeshProUGUI rightForceText;
    [SerializeField] TextMeshProUGUI leftForceText;
    [SerializeField] TextMeshProUGUI fuelConsumptionText;

    Part partSelected;
    [SerializeField]
    RectTransform partTextPanel;
    [SerializeField]
    TextMeshProUGUI partText;
    [SerializeField]
    RectTransform partTextLineParent;

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
            partTextPanel.gameObject.SetActive(false);
            if (partSelected != null)
            {
                partSelected.SetOutline(false);
                partSelected = null;
            }
            return;
        }

        SelectPart();

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


        massPosObj.transform.position = player.massCenterWorldPos;
    }

    public void StartConstructMode()
    {
        foreach (Part part in PartSpawner.instance.floatingPartList)
        {
            if ((part.transform.position - player.transform.position).magnitude < constructRange)
            {
                SetConstructingParts(part);
            }
            else
            {
                part.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }

        isConstructMode = true;
        panelConstructScreen.SetActive(true);
        buttonConstruct.SetActive(false);
    }

    public void EndConstructMode()
    {
        foreach (Part part in PartSpawner.instance.floatingPartList)
        {
            part.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        }

        foreach (Part part in partsConstructingList)
        {
            part.EndConstructMode();
        }
        partsConstructingList.Clear();

        isConstructMode = false;
        panelConstructScreen.SetActive(false);
        buttonConstruct.SetActive(true);
    }

    public void SetConstructingParts(Part part)
    {
        partsConstructingList.Add(part);
        part.StartConstructMode();
    }

    void SelectPart()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -10;

        // レイを飛ばす
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        // レイキャストがパーツに当たった場合
        if (hit.collider != null && hit.collider.gameObject.GetComponent<Part>())
        {
            Part part = hit.collider.gameObject.GetComponent<Part>();
            if (part != partSelected)
            {
                if (partSelected != null) partSelected.SetOutline(false);
                partSelected = part;
                partSelected.SetOutline(true);
            }
        }

        // 説明を出す
        if (partSelected != null)
        {
            partTextPanel.gameObject.SetActive(true);
            partTextPanel.transform.position = partSelected.transform.position;
            Vector3 movePos = partTextPanel.transform.localPosition;
            Vector3 scale = Vector3.one;
            if (movePos.x < 500)
            {
                movePos.x += 330;
            }
            else
            {
                movePos.x -= 330;
                scale.x = -1;
            }
            if (movePos.y < 200)
            {
                movePos.y += 200;
            }
            else
            {
                movePos.y -= 200;
                scale.y = -1;
            }
            partTextPanel.transform.localPosition = movePos;
            partTextLineParent.transform.localScale = scale;
            partText.text = $"名前：{partSelected}\n";
        }
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

    public void SetMassText(float mass)
    {
        massText.text = "総重量： " + mass.ToString("F2");
    }

    public void SetFuelVolumeText(float fuelVolume)
    {
        fuelVolumeText.text = "燃料容量： " + fuelVolume.ToString("F2");
    }

    public void SetRotationForceText(float rightForce, float leftForce)
    {
        rightForceText.text = "右回転力： " + rightForce.ToString("F2");
        leftForceText.text = "左回転力： " + leftForce.ToString("F2");
    }

    public void SetFuelConsumptionText(float fuelConsumption)
    {
        fuelConsumptionText.text = "総燃料消費： " + fuelConsumption.ToString("F2") + "/s";
    }
}
