using UnityEngine;

public class FirstPersonAnimator : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] Jump jump;

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        groundCheck = GetComponentInChildren<GroundCheck>();
        jump = GetComponent<Jump>();
    }

    void OnEnable()
    {
        if (jump != null)
            jump.Jumped += OnJump; // Subscribe to jump event
    }

    void OnDisable()
    {
        if (jump != null)
            jump.Jumped -= OnJump; // Unsubscribe
    }

    void OnJump()
    {
        // Fire jump trigger once
        animator.SetTrigger("Jump");
    }
}
