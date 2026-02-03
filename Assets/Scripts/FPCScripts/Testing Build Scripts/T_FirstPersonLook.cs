using UnityEngine;

public class T_FirstPersonLook : MonoBehaviour
{
    [Header("References")]
    public Transform character;        // the player body
    public Camera playerCamera;        // the local player's camera
    public GameObject playerBody;      // for hiding body mesh

    [Header("Look Settings")]
    public float sensitivity = 2f;
    public float smoothing = 1.5f;

    [Header("Tweaks")]
    public bool hideBody = true;

    [Header("Input Control")]
    public bool canLook = true;

    private Vector2 velocity;
    private Vector2 frameVelocity;

    void Start()
    {

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetBodyVisible(!hideBody);

        if (character == null)
            character = GetComponentInParent<FirstPersonMovement>().transform;
    }

    void Update()
    {

        if (!canLook) return;

        // Mouse input
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;

        // Clamp vertical rotation
        velocity.y = Mathf.Clamp(velocity.y, -90f, 90f);

        // Apply rotations
        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
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
