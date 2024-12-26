using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Frame : Part
{
    public List<Part> connectedParts = new List<Part>();

    public void AddConnectedPart(Part part)
    {
        connectedParts.Add(part);

    }

    public void RemoveConnectedPart(Part part)
    {
        connectedParts.Remove(part);
    }

    /*
        public void AddConnectedToChildPoint(ConnectPoint connectPoint)
        {
            connectedToChildPoins.Add(connectPoint);
        }

        public void RemoveConnectedToChildPoint(ConnectPoint connectPoint)
        {
            connectedToChildPoins.Remove(connectPoint);
        }
        */
}
