using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Wichtig für TextMeshPro

public class MainMenuController : MonoBehaviour
{
    // Name der ersten Spielszene, die geladen werden soll.
    // Passe diesen Namen im Unity Inspector an, falls deine Szene anders heißt.
    [SerializeField] private string settingsSceneName = "SettingsScene";
    [SerializeField] private string firstLevelSceneName = "SampleScene";

    [Header("UI Panels")]
    [SerializeField] private GameObject nameInputPanel; // Panel für die Namenseingabe
    [SerializeField] private GameObject mainMenuPanel; // Panel mit den Hauptmenü-Buttons
    [SerializeField] private GameObject memeGalleryPanel; // Panel für die Meme-Galerie
    [SerializeField] private GameObject galleryBackgroundPanel; // NEU: Ausgegrauter Hintergrund für die Galerie

    [SerializeField] private TMP_InputField nameInputField; // Referenz zum Eingabefeld

    [Header("Display Elements")]
    [SerializeField] private TextMeshProUGUI playerNameText; // Textfeld zur Anzeige des Spielernamens

    [Header("Audio")]
    [SerializeField] private AudioClip backgroundMusic;

    private void Start()
    {
        MusicManager.instance.PlayMusic(backgroundMusic);

        // Stelle sicher, dass die Galerie und ihr Hintergrund zu Beginn ausgeblendet sind.
        memeGalleryPanel.SetActive(false);
        galleryBackgroundPanel.SetActive(false);

        // Prüfe, ob ein Spielername bereits vorhanden ist.
        if (PlayerProfile.instance != null && !string.IsNullOrWhiteSpace(PlayerProfile.instance.PlayerName))
        {
            // Name ist vorhanden: Zeige das Hauptmenü und verstecke die Namenseingabe.
            ShowMainMenu();
        }
        else
        {
            // Kein Name vorhanden: Zeige die Namenseingabe und verstecke das Hauptmenü.
            ShowNameInput();
        }
    }

    public void StartGame()
    {
        // Diese Methode wird jetzt nur noch vom normalen Hauptmenü aufgerufen.
        // Die Namensspeicherung passiert in einer eigenen Methode.
        SceneController.instance.LoadScene(firstLevelSceneName);
    }

    public void SaveNameAndShowMenu()
    {
        // Prüfe, ob überhaupt ein Name eingegeben wurde.
        if (string.IsNullOrWhiteSpace(nameInputField.text))
        {
            Debug.LogWarning("Bitte gib einen Namen ein.");
            // Optional: Zeige hier eine UI-Warnung an.
            return; // Breche ab, wenn kein Name da ist.
        }

        // Speichere den Namen aus dem Input-Feld.
        PlayerProfile.instance.SetPlayerName(nameInputField.text);
        Debug.Log($"Spielername gesetzt auf: {PlayerProfile.instance.PlayerName}");
        
        // Wechsle zum Hauptmenü-Panel, anstatt das Spiel zu starten.
        ShowMainMenu();
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

    private void ShowNameInput()
    {
        nameInputPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    private void ShowMainMenu()
    {
        nameInputPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        // Aktualisiere die Namensanzeige, wenn das Hauptmenü gezeigt wird.
        if (PlayerProfile.instance != null && playerNameText != null)
        {
            playerNameText.text = "Playing as: " + PlayerProfile.instance.PlayerName;
            // Stelle sicher, dass das Textfeld sichtbar ist.
            playerNameText.gameObject.SetActive(true);
        }
    }

    public void OpenMemeGallery()
    {
        mainMenuPanel.SetActive(false);
        galleryBackgroundPanel.SetActive(true); // Hintergrund aktivieren
        memeGalleryPanel.SetActive(true);
        // Das Panel füllt sich selbst dank der OnEnable-Methode im MemeGalleryController.
    }

    public void CloseMemeGallery()
    {
        memeGalleryPanel.SetActive(false);
        galleryBackgroundPanel.SetActive(false); // Hintergrund deaktivieren
        mainMenuPanel.SetActive(true);
    }
}
