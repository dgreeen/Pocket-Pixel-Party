using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script's only job is to load the main menu scene
/// after the persistent managers in this boot scene have been initialized.
/// </summary>
public class Bootstrapper : MonoBehaviour
{
    // Set this to the name of your main menu scene in the Inspector.
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        // Load the main menu scene.
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
