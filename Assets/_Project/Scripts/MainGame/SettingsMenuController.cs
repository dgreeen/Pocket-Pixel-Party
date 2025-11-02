using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenuController : MonoBehaviour
{
    [Tooltip("Der Name der Hauptmenü-Szene, zu der zurückgekehrt werden soll.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Passe den Namen an, falls deine Szene anders heißt

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Hier werden wir später die Logik für die Einstellungen (z.B. Lautstärke) hinzufügen.
}
