using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

/// <summary>
/// Bietet einfache Methoden zur Szenen-Navigation.
/// </summary>
public class SceneNavigation : MonoBehaviour
{
    /// <summary>
    /// Der Name der Hauptmenü-Szene.
    /// </summary>
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// Lädt die Szene mit dem angegebenen Namen.
    /// Stelle sicher, dass die Szene in den Build Settings eingetragen ist.
    /// </summary>
    /// <param name="sceneName">Der Name der zu ladenden Szene.</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Lädt die Hauptmenü-Szene.
    /// </summary>
    public void LoadMainMenuScene()
    {
        // Prüfen, ob ein SceneController existiert, um die Position zu speichern
        if (SceneController.instance != null)
        {
            SceneController.instance.ReturnToMainMenuAndSavePosition();
        }
        else
        {
            // Fallback, falls kein SceneController da ist (z.B. in einem Minispiel ohne Rückkehrpunkt)
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}