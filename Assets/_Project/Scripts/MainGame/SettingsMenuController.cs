using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Der Name der Hauptmenü-Szene.")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Player Settings")]
    [Tooltip("Das Eingabefeld für den Spielernamen.")]
    [SerializeField] private TMP_InputField nameInputField;

    [Header("Audio Settings")]
    [Tooltip("Der AudioMixer, der die Lautstärke regelt.")]
    [SerializeField] private AudioMixer audioMixer;
    [Tooltip("Der Slider für die Gesamtlautstärke.")]
    [SerializeField] private Slider masterVolumeSlider;

    // Der Name des Parameters, den wir im AudioMixer verfügbar machen.
    private const string MASTER_VOLUME_KEY = "MasterVolume";

    private void Start()
    {
        // Lade die gespeicherte Lautstärke und setze den Slider-Wert.
        // Die Lautstärke selbst wurde bereits beim Spielstart durch den GameInitializer gesetzt.
        LoadVolumeSetting();

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
        // Wir verwenden den SceneController, um zur vorherigen Szene zurückzukehren.
        if (SceneController.instance != null)
        {
            SceneController.instance.ReturnToPreviousScene();
        }
    }

    /// <summary>
    /// Wird vom Lautstärke-Slider aufgerufen, wenn sich sein Wert ändert.
    /// </summary>
    /// <param name="volume">Der Wert des Sliders (zwischen 0.0001 und 1.0).</param>
    public void SetMasterVolume(float volume)
    {
        // Wir fangen den Wert 0 ab, da Log10(0) negativ unendlich ist.
        // Stattdessen verwenden wir einen sehr kleinen Wert, der den Mixer effektiv stummschaltet (-80dB).
        float safeVolume = Mathf.Clamp(volume, 0.0001f, 1.0f);

        // AudioMixer verwendet eine logarithmische Skala (Dezibel).
        // Wir konvertieren den linearen Slider-Wert in einen logarithmischen dB-Wert.
        audioMixer.SetFloat(MASTER_VOLUME_KEY, Mathf.Log10(safeVolume) * 20);

        // Speichere die Einstellung im Spielerprofil.
        if (PlayerProfile.instance != null)
        {
            PlayerProfile.instance.SetMasterVolume(volume);
        }
    }

    /// <summary>
    /// Wird vom "Speichern"-Button aufgerufen, um den Namen zu ändern.
    /// </summary>
    public void ChangePlayerName()
    {
        if (PlayerProfile.instance != null && nameInputField != null && !string.IsNullOrWhiteSpace(nameInputField.text))
        {
            PlayerProfile.instance.SetPlayerName(nameInputField.text);
            Debug.Log($"Spielername geändert zu: {nameInputField.text}");
            // Optional: Gib dem Spieler visuelles Feedback, z.B. "Gespeichert!".
        }
    }

    /// <summary>
    /// Wird vom "Neuer Spieler"-Button aufgerufen. Löscht das Profil und kehrt zum Hauptmenü zurück.
    /// </summary>
    public void ResetGame()
    {
        if (PlayerProfile.instance != null)
        {
            PlayerProfile.instance.DeleteProfile();
        }

        // Kehre zum Hauptmenü zurück. Da kein Profil mehr existiert, wird die Namenseingabe erscheinen.
        SceneController.instance.LoadScene(mainMenuSceneName);
    }
}
