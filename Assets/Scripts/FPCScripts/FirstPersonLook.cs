using UnityEngine;
using Photon.Pun;

public class FirstPersonLook : MonoBehaviourPun
{
    [Header("References")]
    public Transform character;
    public Camera playerCamera;
    public GameObject playerBody;

    [Header("Look Settings")]
    public float sensitivity = 2f;
    public float smoothing = 1.5f;

    [Header("Tweaks")]
    public bool hideBody = true;

    [Header("Input Control")]
    public bool canLook = true;

    private Vector2 velocity;
    private Vector2 frameVelocity;

    // Rotation lock
    private bool rotationLocked = false;
    private Quaternion lockedCameraRotation;
    private Quaternion lockedCharacterRotation;

    PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            this.enabled = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetBodyVisible(!hideBody);

        if (character == null)
            character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        // If locked, force the saved rotation every frame regardless of any input
        if (rotationLocked)
        {
            transform.localRotation = lockedCameraRotation;
            character.localRotation = lockedCharacterRotation;
            return;
        }

        if (!canLook) return;

        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;

        velocity.y = Mathf.Clamp(velocity.y, -90f, 90f);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }

    public void LockRotation()
    {
        rotationLocked = true;
        lockedCameraRotation = transform.localRotation;
        lockedCharacterRotation = character.localRotation;
    }

    public void UnlockRotation()
    {
        rotationLocked = false;
        // Sync velocity to match the locked rotation so there's no snap when resuming
        velocity.y = -transform.localRotation.eulerAngles.x;
        if (velocity.y < -90f) velocity.y += 360f;
        velocity.x = character.localRotation.eulerAngles.y;
        frameVelocity = Vector2.zero;
    }

    public void SetBodyVisible(bool visible)
    {
        if (playerBody == null) return;

        foreach (var smr in playerBody.GetComponentsInChildren<SkinnedMeshRenderer>())
            smr.enabled = visible;
        foreach (var mr in playerBody.GetComponentsInChildren<MeshRenderer>())
            mr.enabled = visible;
    }

    public void ToggleBodyVisibility()
    {
        hideBody = !hideBody;
        SetBodyVisible(!hideBody);
    }
}