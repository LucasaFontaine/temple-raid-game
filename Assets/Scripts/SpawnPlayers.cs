using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;
    
    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    void Start()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(minX, maxX),
            transform.position.y,
            Random.Range(minZ, maxZ)
        );
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
    }
}
