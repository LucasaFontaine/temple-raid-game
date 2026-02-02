using UnityEngine;

[ExecuteInEditMode]
public class GroundCheck : MonoBehaviour
{
    [Tooltip("Maximum distance from the ground.")]
    public float distanceThreshold = 0.15f;

    [Tooltip("Whether this transform is grounded now.")]
    public bool isGrounded = true;

    /// <summary>
    /// Called when the ground is touched again.
    /// </summary>
    public event System.Action Grounded;

    [Header("Jump Delay")]
    [Tooltip("Time to ignore ground right after jump to allow animation to play.")]
    public float jumpGroundedDelay = 0.2f;

    private float jumpTimer = 0f;

    const float OriginOffset = 0.001f;
    Vector3 RaycastOrigin => transform.position + Vector3.up * OriginOffset;
    float RaycastDistance => distanceThreshold + OriginOffset;

    void LateUpdate()
    {
        // Countdown the jump timer
        if (jumpTimer > 0f)
            jumpTimer -= Time.deltaTime;

        // Check if we are grounded now
        bool groundedRay = Physics.Raycast(RaycastOrigin, Vector3.down, distanceThreshold * 2);

        // Only count as grounded if the jump timer is finished
        bool isGroundedNow = groundedRay && jumpTimer <= 0f;

        // Call event if we were in the air and we are now touching the ground
        if (isGroundedNow && !isGrounded)
        {
            Grounded?.Invoke();
        }

        // Update isGrounded
        isGrounded = isGroundedNow;
    }

    // temporarily prevent the grounded check.
    public void NotifyJump()
    {
        jumpTimer = jumpGroundedDelay;
        isGrounded = false; // immediately mark as airborne
    }

    void OnDrawGizmosSelected()
    {
        // Draw a line in the Editor to show whether we are touching the ground
        Debug.DrawLine(RaycastOrigin, RaycastOrigin + Vector3.down * RaycastDistance, isGrounded ? Color.white : Color.red);
    }
}
