using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;

    [Header("Animations")]
    [SerializeField] private Animator _animator;

    Rigidbody rigidbody;

    /// <summary>
    /// Functions to override movement speed. Will use the last added override.
    /// </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get target moving speed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get target velocity from input.
        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        Vector3 worldVelocity = transform.rotation * new Vector3(
            input.x * targetMovingSpeed,
            rigidbody.linearVelocity.y,
            input.y * targetMovingSpeed
        );

        // Apply movement.
        rigidbody.linearVelocity = worldVelocity;

        // -------- ANIMATION DATA --------

        // Convert world velocity to local space
        Vector3 localVelocity =
            transform.InverseTransformDirection(rigidbody.linearVelocity);

        // Normalize for blend tree (-1 to 1)
        float maxSpeed = IsRunning ? runSpeed : speed;

        float forward = Mathf.Clamp(localVelocity.z / maxSpeed, -1f, 1f);
        float strafe  = Mathf.Clamp(localVelocity.x / maxSpeed, -1f, 1f);

        _animator.SetFloat("forward", forward);
        _animator.SetFloat("strafe", strafe);
    }
}
