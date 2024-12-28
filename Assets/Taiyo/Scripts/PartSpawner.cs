using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PartSpawner : MonoBehaviour
{
    public static PartSpawner instance;

    [SerializeField] Transform floatingPartFolder;
    [SerializeField] public List<Part> floatingPartList = new List<Part>();

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
        foreach (Part part in floatingPartFolder.GetComponentsInChildren<Part>())
        {
            floatingPartList.Add(part);
        }
    }

    public void AddFloatingPart(Part part)
    {
        if (floatingPartList.Contains(part))
        {
            return;
        }
        floatingPartList.Add(part);
        part.transform.SetParent(floatingPartFolder);
    }

    public void RemoveFloatingPart(Part part)
    {
        floatingPartList.Remove(part);
    }
}
