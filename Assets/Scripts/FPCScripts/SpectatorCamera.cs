using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

public class SpectatorCamera : MonoBehaviour
{
    [Header("Orbit Settings")]
    public float distance = 4f;
    public float orbitSpeed = 3f;
    public float verticalMin = -20f;
    public float verticalMax = 60f;
    public float smoothSpeed = 5f;

    [Header("Target Offset")]
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f); // focus on upper body

    private List<Transform> targets = new List<Transform>();
    private int currentTargetIndex = 0;

    private float yaw = 0f;
    private float pitch = 20f;

    private Camera spectatorCam;

    void Awake()
    {
        spectatorCam = GetComponent<Camera>();
        if (spectatorCam == null)
            spectatorCam = gameObject.AddComponent<Camera>();
    }

    public void StartSpectating()
    {
        gameObject.SetActive(true);
        RefreshTargets();

        if (targets.Count == 0)
        {
            Debug.LogWarning("No players to spectate.");
            return;
        }

        currentTargetIndex = 0;
    }

    public void StopSpectating()
    {
        gameObject.SetActive(false);
    }

    // refresh the list of living players
    public void RefreshTargets()
    {
        targets.Clear();

        // find all players except self
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.IsLocal) continue;

            // find their game object w actor number tag or name
            GameObject playerObj = FindPlayerObject(player.ActorNumber);
            if (playerObj != null)
                targets.Add(playerObj.transform);
        }
    }

    GameObject FindPlayerObject(int actorNumber)
    {
        foreach (var pv in FindObjectsByType<PhotonView>(FindObjectsSortMode.None))
        {
            if (pv.Owner != null && pv.Owner.ActorNumber == actorNumber && pv.CompareTag("Player"))
                return pv.gameObject;
        }
        return null;
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (targets.Count == 0) return;

        // Switch targets with Q and E
        if (Input.GetKeyDown(KeyCode.E))
            currentTargetIndex = (currentTargetIndex + 1) % targets.Count;
        if (Input.GetKeyDown(KeyCode.Q))
            currentTargetIndex = (currentTargetIndex - 1 + targets.Count) % targets.Count;

        // Remove null targets (players who disconnected)
        targets.RemoveAll(t => t == null);
        if (targets.Count == 0) return;

        Transform target = targets[currentTargetIndex];

        // Orbit input
        yaw += Input.GetAxis("Mouse X") * orbitSpeed;
        pitch -= Input.GetAxis("Mouse Y") * orbitSpeed;
        pitch = Mathf.Clamp(pitch, verticalMin, verticalMax);

        // Calculate position
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPos = target.position + targetOffset;
        Vector3 desiredPos = targetPos + rotation * new Vector3(0f, 0f, -distance);

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * smoothSpeed);
        transform.LookAt(targetPos);
    }
}