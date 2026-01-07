using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Load the main game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
