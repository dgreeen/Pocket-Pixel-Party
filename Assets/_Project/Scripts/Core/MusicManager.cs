using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource), typeof(AudioListener))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource audioSource;

    [Header("Audio Mixer Group")]
    [Tooltip("Ziehe hier die 'Master' Audio Mixer Group hinein.")]
    [SerializeField] private AudioMixerGroup masterMixerGroup;

    void Awake()
    {
        // Klassisches Singleton-Pattern
        if (instance == null)
        {
            instance = this;

            // DontDestroyOnLoad funktioniert nur bei Root-GameObjects.
            // Wir stellen sicher, dass dieses Objekt ein Root-Objekt ist, indem wir es entkoppeln.
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);

            // Hole die AudioSource-Komponente und konfiguriere sie
            audioSource = GetComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = masterMixerGroup; // Wichtig für die Lautstärkeregelung!
            audioSource.loop = true; // Die Musik soll in einer Schleife laufen
        }
        else
        {
            // Wenn bereits ein MusicManager existiert, zerstöre diesen hier.
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        // Spiele nur neue Musik, wenn sich der Clip geändert hat
        if (audioSource.clip == musicClip) return;

        audioSource.clip = musicClip;
        audioSource.Play();
    }
}
