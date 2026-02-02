using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject container;
    private bool isPaused = false;

    [Header("Player Reference")]
    public FirstPersonMovement playerMovement;
    public FirstPersonLook playerLook;

    void Start()
    {
        container.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        container.SetActive(true);

        // Cursor unlock
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable player movement
        PlayerInputDisabled(true);

        // Ensure EventSystem exists
        if (EventSystem.current == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        // Optional: select first button automatically
        if (container.transform.childCount > 0)
        {
            EventSystem.current.SetSelectedGameObject(container.transform.GetChild(0).gameObject);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        container.SetActive(false);

        // Cursor lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Enable player movement
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
