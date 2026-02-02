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

    void Update()
    {
        animator.SetBool("Grounded", groundCheck.isGrounded);
    }

    void OnEnable()
    {
        if (jump != null)
            jump.Jumped += OnJump;
    }

    void OnDisable()
    {
        if (jump != null)
            jump.Jumped -= OnJump;
    }

    void OnJump()
    {
        animator.SetTrigger("Jump");

        if (groundCheck != null)
            groundCheck.NotifyJump(); // prevent ground detection for a short time
    }
}
