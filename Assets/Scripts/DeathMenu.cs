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

        if (playerHealth != null)
            playerHealth.Died += OnPlayerDied;
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.Died -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        if (isDead) return;
        isDead = true;

        // Disable input FIRST
        PlayerInputDisabled(true);

        // Lock the camera in place before unlocking cursor
        if (playerLook != null)
            playerLook.LockRotation();

        // Then unlock cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        container.SetActive(true);

        if (EventSystem.current == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        if (container.transform.childCount > 0)
            EventSystem.current.SetSelectedGameObject(container.transform.GetChild(0).gameObject);
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