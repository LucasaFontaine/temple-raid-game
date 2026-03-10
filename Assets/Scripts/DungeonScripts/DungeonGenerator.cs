using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Room Prefabs")]
    public List<GameObject> roomPrefabs;
    public GameObject startRoomPrefab;
    public GameObject startHallwayPrefab;
    public GameObject endRoomPrefab;

    [Header("Dead End Rooms")]
    public List<GameObject> deadEndRoomPrefabs;
    public int minRoomsBeforeDeadEnd = 3;
    public GameObject doorwayWallPrefab;
    public float doorwayWallYOffset = 3f;

    [Header("Generation Settings")]
    public int minRooms = 8;
    public int maxRooms = 15;
    public int maxAttempts = 30;

    [Header("Boundary")]
    public float boundaryZ = 0f; // rooms cannot generate past
    public bool useBoundary = true;

    [Header("Debug")]
    public bool disableOverlapCheck = false;

    private List<Room> placedRooms = new List<Room>();
    private Queue<DoorwayPoint> openDoorways = new Queue<DoorwayPoint>();
    private int roomCount = 0;
    private int targetRoomCount;

    IEnumerator Start()
    {
        int seed;

        if (PhotonNetwork.IsMasterClient)
        {
            seed = Random.Range(0, 999999);
            PhotonNetwork.CurrentRoom.SetCustomProperties(
                new ExitGames.Client.Photon.Hashtable { { "dungeonSeed", seed } }
            );
        }
        else
        {
            while (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("dungeonSeed"))
                yield return null;

            seed = (int)PhotonNetwork.CurrentRoom.CustomProperties["dungeonSeed"];
        }

        Random.InitState(seed);

        targetRoomCount = Random.Range(minRooms, maxRooms + 1);
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        GameObject startObj = Instantiate(startRoomPrefab, transform.position, transform.rotation);
        Room startRoom = startObj.GetComponent<Room>();
        placedRooms.Add(startRoom);
        roomCount++;

        if (startHallwayPrefab != null)
            {
                List<DoorwayPoint> startDoorways = startRoom.GetOpenDoorways();
                if (startDoorways.Count > 0)
                {
                    DoorwayPoint firstDoorway = startDoorways[0];
                    bool hallwayPlaced = TryPlaceRoom(startHallwayPrefab, firstDoorway);
                    if (!hallwayPlaced)
                        Debug.LogWarning("Failed to place start hallway!");
                }
            }

            // Queue remaining open doorways for normal generation
        foreach (var room in placedRooms)
            foreach (var doorway in room.GetOpenDoorways())
                if (!openDoorways.Contains(doorway))
                    openDoorways.Enqueue(doorway);
    
        while (roomCount < targetRoomCount && openDoorways.Count > 0)
        {
            DoorwayPoint currentDoorway = openDoorways.Dequeue();

            bool placed = false;

            List<GameObject> pool = new List<GameObject>(roomPrefabs);
            if (roomCount >= minRoomsBeforeDeadEnd && deadEndRoomPrefabs.Count > 0)
                pool.AddRange(deadEndRoomPrefabs);

            List<GameObject> shuffled = ShuffleList(pool);

            for (int attempt = 0; attempt < maxAttempts && !placed; attempt++)
            {
                GameObject prefab = shuffled[attempt % shuffled.Count];
                placed = TryPlaceRoom(prefab, currentDoorway);
            }

            if (!placed)
                SealDoorway(currentDoorway);
        }

        if (openDoorways.Count > 0)
        {
            DoorwayPoint lastDoorway = openDoorways.Dequeue();
            bool endPlaced = TryPlaceRoom(endRoomPrefab, lastDoorway);
            if (!endPlaced)
                SealDoorway(lastDoorway);
        }

        while (openDoorways.Count > 0)
        {
            DoorwayPoint remaining = openDoorways.Dequeue();
            SealDoorway(remaining);
        }
    }

    bool TryPlaceRoom(GameObject prefab, DoorwayPoint targetDoorway)
    {
        GameObject newObj = Instantiate(prefab);
        Room newRoom = newObj.GetComponent<Room>();

        if (newRoom == null || newRoom.doorways.Count == 0)
        {
            Destroy(newObj);
            return false;
        }

        List<DoorwayPoint> newDoorways = newRoom.doorways;
        DoorwayPoint connectingDoorway = newDoorways[Random.Range(0, newDoorways.Count)];

        AlignRoom(newObj, connectingDoorway, targetDoorway);

        if (useBoundary && newObj.transform.position.z < boundaryZ)
        {
            Destroy(newObj);
            return false;
        }

        if (RoomOverlaps(newRoom))
        {
            Destroy(newObj);
            return false;
        }

        connectingDoorway.isConnected = true;
        targetDoorway.isConnected = true;

        placedRooms.Add(newRoom);
        roomCount++;

        foreach (var d in newRoom.GetOpenDoorways())
            openDoorways.Enqueue(d);

        return true;
    }

    void AlignRoom(GameObject roomObj, DoorwayPoint roomDoor, DoorwayPoint targetDoor)
    {
        Quaternion targetRotation = targetDoor.transform.rotation * Quaternion.Euler(0, 180f, 0);
        Quaternion rotationDiff = targetRotation * Quaternion.Inverse(roomDoor.transform.rotation);
        roomObj.transform.rotation = rotationDiff * roomObj.transform.rotation;

        Vector3 positionDiff = targetDoor.transform.position - roomDoor.transform.position;
        roomObj.transform.position += positionDiff;
    }

    bool RoomOverlaps(Room room)
    {
        if (disableOverlapCheck) return false;

        if (room.roomBounds == null)
        {
            Debug.LogWarning("Room has no roomBounds: " + room.gameObject.name);
            return false;
        }

        Vector3 newCenter = room.roomBounds.transform.TransformPoint(room.roomBounds.center);
        Vector3 newSize = Vector3.Scale(room.roomBounds.size, room.roomBounds.transform.lossyScale);

        foreach (Room placed in placedRooms)
        {
            if (placed.roomBounds == null) continue;

            Vector3 placedCenter = placed.roomBounds.transform.TransformPoint(placed.roomBounds.center);
            Vector3 placedSize = Vector3.Scale(placed.roomBounds.size, placed.roomBounds.transform.lossyScale);

            bool overlapX = Mathf.Abs(newCenter.x - placedCenter.x) < (newSize.x * 0.5f + placedSize.x * 0.5f);
            bool overlapY = Mathf.Abs(newCenter.y - placedCenter.y) < (newSize.y * 0.5f + placedSize.y * 0.5f);
            bool overlapZ = Mathf.Abs(newCenter.z - placedCenter.z) < (newSize.z * 0.5f + placedSize.z * 0.5f);

            if (overlapX && overlapY && overlapZ)
                return true;
        }

        return false;
    }

    void SealDoorway(DoorwayPoint doorway)
    {
        doorway.isConnected = true;
        if (doorwayWallPrefab != null)
        {
            Vector3 spawnPos = doorway.transform.position;
            spawnPos.y = doorway.transform.position.y + doorwayWallYOffset;
            Instantiate(doorwayWallPrefab, spawnPos, doorway.transform.rotation);
        }
    }

    List<GameObject> ShuffleList(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            GameObject temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }
}