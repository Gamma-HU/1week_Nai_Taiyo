using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Frame : Part
{
    public List<Part> connectedParts = new List<Part>();

    protected override void Awake()
    {
        base.Awake();
        transform.localPosition += new Vector3(0, 0, 1); //他のオブジェクトよりも奥に
    }

    public void AddConnectedPart(Part part)
    {
        connectedParts.Add(part);

    }

    public void RemoveConnectedPart(Part part)
    {
        connectedParts.Remove(part);
    }

    public override void Damage(float damage)
    {
        int childCount = 0;
        foreach (Part part in connectedParts)
        {
            if (!part.isCockpit)
            {
                childCount++;
            }
        }

        if (childCount > 0)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Part part = connectedParts[i];
                if (!part.isCockpit)
                {
                    part.Damage(damage / childCount);
                }
            }
        }
        else
        {
            base.Damage(damage);
        }
    }
}
