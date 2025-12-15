using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Der Name der Hauptmenue-Szene.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Player Settings")]
    [Tooltip("Das Eingabefeld fuer den Spielernamen.")]
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Audio Settings")]
    [Tooltip("Der AudioMixer, der die Lautstaerke regelt.")]
    [SerializeField] private AudioMixer audioMixer;
    [Tooltip("Der Slider fuer die Gesamtlautstaerke.")]
    [SerializeField] private Slider masterVolumeSlider;

    // Der Name des Parameters, den wir im AudioMixer verfuegbar machen.
    private const string MASTER_VOLUME_KEY = "MasterVolume";

    private void Start()
    {
        // Lade die gespeicherte Lautstaerke und setze den Slider-Wert.
        // Die Lautstaerke selbst wurde bereits beim Spielstart durch den GameInitializer gesetzt.
        LoadVolumeSetting();
        
        // Fuege den Listener fuer den Slider erst hinzu, NACHDEM der initiale Wert geladen wurde.
        // Dies verhindert, dass das Laden des Wertes ein Speicher-Event ausloest.
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        // Lade den aktuellen Spielernamen in das Eingabefeld.
        LoadPlayerName();
    }

    private void LoadVolumeSetting() {
        // Lade den Wert aus dem PlayerProfile
        if (PlayerProfile.instance != null && masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerProfile.instance.MasterVolume;
        }
    }

    private void LoadPlayerName()
    {
        if (PlayerProfile.instance != null && nameInputField != null)
        {
            // Zeige den aktuellen Namen des Spielers im Eingabefeld an.
            nameInputField.text = PlayerProfile.instance.PlayerName;
        }
    }

    public void BackToMainMenu()
    {
        // Wir verwenden den SceneController, um zur vorherigen Szene zurueckzukehren.
        if (SceneController.instance != null)
        {
            SceneController.instance.ReturnToPreviousScene();
        }
    }

    /// <summary>
    /// Wird vom Lautstaerke-Slider aufgerufen, wenn sich sein Wert aendert.
    /// </summary>
    /// <param name="volume">Der Wert des Sliders (zwischen 0.0001 und 1.0).</param>
    public void SetMasterVolume(float volume)
    {
        // Speichere die Einstellung im Spielerprofil.
        if (PlayerProfile.instance != null)
        {
            // Die Logik zum Anwenden auf den Mixer ist bereits im PlayerProfile,
            // also rufen wir sie dort auf, um den Wert zu setzen und zu speichern.
            PlayerProfile.instance.ApplyMasterVolume(volume);
            PlayerProfile.instance.SetMasterVolume(volume);
        }
    }

    /// <summary>
    /// Wird vom "Speichern"-Button aufgerufen, um den Namen zu aendern.
    /// </summary>
    public void ChangePlayerName()
    {
        if (PlayerProfile.instance != null && nameInputField != null && !string.IsNullOrWhiteSpace(nameInputField.text))
        {
            PlayerProfile.instance.SetPlayerName(nameInputField.text);
            Debug.Log($"Spielername geaendert zu: {nameInputField.text}");
            // Optional: Gib dem Spieler visuelles Feedback, z.B. "Gespeichert!".
        }
    }

    /// <summary>
    /// Wird vom "Neuer Spieler"-Button aufgerufen. Loescht das Profil und kehrt zum Hauptmenue zurueck.
    /// </summary>
    public void ResetGame()
    {
        if (PlayerProfile.instance != null)
        {
            PlayerProfile.instance.DeleteProfile();
        }

        // Kehre zum Hauptmenue zurueck. Da kein Profil mehr existiert, wird die Namenseingabe erscheinen.
        SceneController.instance.LoadScene(mainMenuSceneName);
    }
}
