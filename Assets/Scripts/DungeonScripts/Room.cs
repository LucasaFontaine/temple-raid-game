using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    public List<DoorwayPoint> doorways = new List<DoorwayPoint>();
    public BoxCollider roomBounds; // assign the RoomBounds child in the Inspector

    void Awake()
    {
        doorways.Clear();
        foreach (Transform child in transform)
        {
            DoorwayPoint dp = child.GetComponent<DoorwayPoint>();
            if (dp != null)
            {
                dp.isConnected = false;
                doorways.Add(dp);
            }
        }
    }

    public List<DoorwayPoint> GetOpenDoorways()
    {
        List<DoorwayPoint> open = new List<DoorwayPoint>();
        foreach (var d in doorways)
            if (!d.isConnected) open.Add(d);
        return open;
    }
}