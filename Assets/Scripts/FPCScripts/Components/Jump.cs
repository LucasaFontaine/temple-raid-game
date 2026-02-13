using UnityEngine;
using Photon.Pun;

public class Jump : MonoBehaviourPun
{
    Rigidbody rb;
    public float jumpStrength = 2;
    public event System.Action Jumped;

    [SerializeField, Tooltip("Prevents jumping when the transform is in mid-air.")]
    GroundCheck groundCheck;


    void Reset()
    {
        // Try to get groundCheck.
        groundCheck = GetComponentInChildren<GroundCheck>();
    }

    void Awake()
    {
        if (!photonView.IsMine)
        {
            // completely disable this script on remote players
            this.enabled = false;
        }
        // Get rigidbody.
        rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        if (Input.GetButtonDown("Jump") && (!groundCheck || groundCheck.isGrounded))
        {
            // Trigger animation first
            Jumped?.Invoke();  

            // Then apply physics
            rb.AddForce(Vector3.up * 100 * jumpStrength);
        }
    }
}