using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Photon.Pun;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    private bool isPaused = false;

    [Header("Player Reference")]
    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;

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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        container.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerInputDisabled(true);

        // Lock the camera in place
        if (playerLook != null)
            playerLook.LockRotation();

        if (EventSystem.current == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        if (container.transform.childCount > 0)
            EventSystem.current.SetSelectedGameObject(container.transform.GetChild(0).gameObject);
    }

    public void ResumeGame()
    {
        isPaused = false;
        container.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Unlock the camera before re-enabling input
        if (playerLook != null)
            playerLook.UnlockRotation();

        PlayerInputDisabled(false);
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void PlayerInputDisabled(bool disabled)
    {
        if (playerMovement != null)
            playerMovement.enabled = !disabled;

        if (playerLook != null)
            playerLook.canLook = !disabled;
    }
}