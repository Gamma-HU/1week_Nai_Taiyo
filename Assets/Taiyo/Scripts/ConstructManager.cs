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

    bool isConstructMode;
    Part partPicking;

    [SerializeField] GameObject panelConstructScreen;
    [SerializeField] TextMeshProUGUI massText;
    [SerializeField] GameObject massPosObj;
    [SerializeField] TextMeshProUGUI fuelVolumeText;
    [SerializeField] TextMeshProUGUI rightForceText;
    [SerializeField] TextMeshProUGUI leftForceText;
    [SerializeField] TextMeshProUGUI fuelConsumptionText;

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
        panelConstructScreen.SetActive(true);
    }

    public void EndConstructMode()
    {
        foreach (Part part in partsConstructingList)
        {
            part.EndConstructMode();
        }
        partsConstructingList.Clear();

        isConstructMode = false;
        panelConstructScreen.SetActive(false);
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

    public void SetMassTextAndPos(float mass, Vector2 pos)
    {
        massText.text = "総重量： " + mass.ToString("F2");
        massPosObj.transform.position = pos;
        Debug.Log(pos);
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
