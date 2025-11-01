using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Name der ersten Spielszene, die geladen werden soll.
    // Passe diesen Namen im Unity Inspector an, falls deine Szene anders heißt.
    [SerializeField] private string firstLevelSceneName = "SampleScene"; 

    public void StartGame()
    {
        // Lädt die erste Spielszene.
        // Stelle sicher, dass SceneController.instance in der nächsten Szene vorhanden ist.
        SceneManager.LoadScene(firstLevelSceneName);
    }

    public void OpenCharacterSelection()
    {
        // Hier könntest du eine Szene für die Charakterauswahl laden.
        // SceneManager.LoadScene("CharacterSelection");
        Debug.Log("Charakterauswahl-Szene öffnen (noch nicht implementiert).");
    }

    public void OpenSettings()
    {
        // Hier könntest du ein Einstellungs-Panel anzeigen oder eine Einstellungs-Szene laden.
        // SceneManager.LoadScene("Settings");
        Debug.Log("Einstellungen öffnen (noch nicht implementiert).");
    }

    public void QuitGame()
    {
        // Beendet die Anwendung. Funktioniert nur im fertigen Build, nicht im Editor.
        Debug.Log("Spiel wird beendet.");
        Application.Quit();
    }
}
