using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(Rigidbody))]
public class FirstPersonMovement : MonoBehaviourPun
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public bool canRun = true;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Animations")]
    [SerializeField] private Animator _animator;

    private Rigidbody rb;
    public bool IsRunning { get; private set; }

    // Optional overrides for speed
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    PhotonView view;
    
    void Awake()
    {   
        view = GetComponent<PhotonView>();
        if (!photonView.IsMine)
        {
            // completely disable this script on remote players
            this.enabled = false;
        }

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        // Check running input
        IsRunning = canRun && Input.GetKey(runKey);

        // Determine speed
        float targetSpeed = IsRunning ? runSpeed : walkSpeed;
        if (speedOverrides.Count > 0)
            targetSpeed = speedOverrides[speedOverrides.Count - 1]();

        // Get input
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Convert to world velocity
        Vector3 move = transform.rotation * new Vector3(input.x * targetSpeed, rb.linearVelocity.y, input.y * targetSpeed);
        rb.linearVelocity = move;

        // Send animation data
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        float maxSpeed = IsRunning ? runSpeed : walkSpeed;
        float forward = Mathf.Abs(localVel.z / maxSpeed) < 0.1f ? 0f : localVel.z / maxSpeed;
        float strafe  = Mathf.Abs(localVel.x / maxSpeed) < 0.1f ? 0f : localVel.x / maxSpeed;

        _animator.SetFloat("forward", Mathf.Clamp(forward, -1f, 1f));
        _animator.SetFloat("strafe", Mathf.Clamp(strafe, -1f, 1f));
    }
}
