using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Name der ersten Spielszene, die geladen werden soll.
    // Passe diesen Namen im Unity Inspector an, falls deine Szene anders heißt.
    [SerializeField] private string settingsSceneName = "SettingsScene";
    [SerializeField] private string firstLevelSceneName = "SampleScene"; 

    [Header("Audio")]
    [SerializeField] private AudioClip backgroundMusic;

    private void Start()
    {
        MusicManager.instance.PlayMusic(backgroundMusic);
    }

    public void StartGame()
    {
        // Lädt die erste Spielszene.
        // Wir verwenden den SceneController, um konsistent zu bleiben.
        // Stelle sicher, dass ein SceneController-Prefab in der MainMenu-Szene ist.
        if (SceneController.instance != null)
            SceneController.instance.LoadScene(firstLevelSceneName);
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
        if (SceneController.instance != null)
            SceneController.instance.LoadScene(settingsSceneName);
    }

    public void QuitGame()
    {
        // Beendet die Anwendung. Funktioniert nur im fertigen Build, nicht im Editor.
        Debug.Log("Spiel beendet");

        // Dieser spezielle Code sorgt dafür, dass das Beenden auch im Unity Editor funktioniert.
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit(); // Dieser Befehl funktioniert nur in einem gebauten Spiel.
        #endif
    }
}
