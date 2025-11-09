using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuController : MonoBehaviour
{

    [Header("Audio Settings")]
    [Tooltip("Der AudioMixer, der die Lautstärke regelt.")]
    [SerializeField] private AudioMixer audioMixer;
    [Tooltip("Der Slider für die Gesamtlautstärke.")]
    [SerializeField] private Slider masterVolumeSlider;

    // Der Name des Parameters, den wir im AudioMixer verfügbar machen.
    private const string MASTER_VOLUME_KEY = "MasterVolume";
    // Der Schlüssel zum Speichern des Werts in den PlayerPrefs.
    private const string MASTER_VOLUME_PREF = "MasterVolumePreference";

    private void Start()
    {
        // Lade die gespeicherte Lautstärke und setze den Slider entsprechend.
        // Der Standardwert ist 1.0f (volle Lautstärke).
        float savedVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_PREF, 1.0f);
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = savedVolume;
        }
        // Wichtig: Setze auch die Lautstärke im Mixer direkt beim Start.
        SetMasterVolume(savedVolume);
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

        // Speichere die Einstellung für zukünftige Spielsitzungen.
        PlayerPrefs.SetFloat(MASTER_VOLUME_PREF, volume);
    }
}
