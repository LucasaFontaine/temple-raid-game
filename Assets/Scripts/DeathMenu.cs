using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Pun;

public class DeathMenu : MonoBehaviour
{
    public GameObject container;

    [Header("Player Reference")]
    public PlayerHealth playerHealth;
    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;

    private bool isDead = false;

    void Awake()
    {
        // Disable this HUD for remote players' prefabs
        PhotonView pv = GetComponentInParent<PhotonView>();
        if (pv != null && !pv.IsMine)
        {
            gameObject.SetActive(false);
            return;
        }
    }

    void Start()
    {
        container.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Subscribe to death event
        if (playerHealth != null)
            playerHealth.Died += OnPlayerDied;
    }

    void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        if (playerHealth != null)
            playerHealth.Died -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        if (isDead) return;
        isDead = true;

        container.SetActive(true);

        // Unlock cursor for UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player controls
        PlayerInputDisabled(true);

        // Ensure EventSystem exists
        if (EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // Auto-select first button (optional)
        if (container.transform.childCount > 0)
        {
            EventSystem.current.SetSelectedGameObject(container.transform.GetChild(0).gameObject);
        }
    }

    public void MainMenuButton()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PhotonDisconnector.DisconnectAndLoadMenu();
    }

    private void PlayerInputDisabled(bool disabled)
    {
        if (playerMovement != null)
            playerMovement.enabled = !disabled;

        if (playerLook != null)
            playerLook.canLook = !disabled;
    }
}
