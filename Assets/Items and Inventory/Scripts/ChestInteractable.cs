using UnityEngine;

public class ChestInteractable : MonoBehaviour
{
    Animator animator;
    bool isOpen;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Debug.LogError("Chest Animator NOT found!", this);
    }

    public void TryOpen(bool hasKey)
    {
        Debug.Log("TryOpen called | HasKey = " + hasKey);

        if (isOpen)
        {
            Debug.Log("Chest already open");
            return;
        }

        if (!hasKey)
        {
            Debug.Log("Player has no key");
            return;
        }

        isOpen = true;
        animator.SetTrigger("Open");
        Debug.Log("Chest animation triggered");
    }
}
