using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; // Wichtig für Dateioperationen
using UnityEngine;
using UnityEngine.Audio;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile instance;

    public event Action<MemeData> OnMemeDataUnlocked;
    public event Action<string> OnCharacterSelected; // NEU: Event für Charakterauswahl

    public string PlayerName { get; private set; }
    public float MasterVolume { get; private set; } = 1.0f; // Standardwert 1.0f

    private readonly HashSet<string> _unlockedMemeIds = new HashSet<string>();
    private string _selectedCharacterId;

    private string _savePath;
    private AudioMixer _mainAudioMixer;

    private void Awake()
    {
        // Singleton-Pattern, um sicherzustellen, dass es nur eine Instanz gibt.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Definiere den Speicherpfad für die JSON-Datei
        _savePath = Path.Combine(Application.persistentDataPath, "playerProfile.json");

        // Finde den AudioMixer robust, anstatt auf eine Zuweisung im Inspector zu hoffen.
        FindMainAudioMixer();

        // Lade das gespeicherte Profil beim Start des Spiels
        LoadProfile();
    }

    private void Start()
    {
        // Wende die geladene Lautstärke an, NACHDEM alle Awake-Methoden und die Audio-Engine-Initialisierung (inkl. Snapshots) abgeschlossen sind.
        ApplyMasterVolume();
    }

    public void SetPlayerName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return;

        PlayerName = newName;
        // Speichere das Profil, wenn der Name geändert wird
        SaveProfile();
    }

    public void SetMasterVolume(float newVolume)
    {
        MasterVolume = Mathf.Clamp(newVolume, 0.0f, 1.0f);
        SaveProfile();
    }

    // NEU: Methode zum Setzen des ausgewählten Charakters
    public void SetSelectedCharacter(string characterId)
    {
        if (string.IsNullOrWhiteSpace(characterId)) return;
        _selectedCharacterId = characterId;
        OnCharacterSelected?.Invoke(characterId);
        SaveProfile();
    }

    public bool UnlockMeme(MemeData memeToUnlock)
    {
        if (memeToUnlock == null || string.IsNullOrEmpty(memeToUnlock.memeId))
        {
            Debug.LogWarning("Versuch, ein ungültiges Meme freizuschalten.");
            return false;
        }

        if (_unlockedMemeIds.Add(memeToUnlock.memeId))
        {
            Debug.Log($"Meme '{memeToUnlock.displayName}' ({memeToUnlock.memeId}) freigeschaltet!");
            OnMemeDataUnlocked?.Invoke(memeToUnlock);
            // Speichere das Profil, wenn ein neues Meme freigeschaltet wird
            SaveProfile();
            return true;
        }
        return false;
    }

    public bool HasUnlockedMeme(string memeId) {
        return _unlockedMemeIds.Contains(memeId);
    }

    // Private Klasse zur Kapselung der zu speichernden Daten
    [Serializable]
    private class PlayerData
    {
        public string playerName;
        public List<string> unlockedMemeIds;
        public float masterVolume;
        public string selectedCharacterId; // NEU: Füge dieses Feld hinzu
    }

    private void SaveProfile()
    {
        // Erstelle ein Datenobjekt mit den aktuellen Spielerdaten
        PlayerData data = new PlayerData
        {
            playerName = PlayerName,
            masterVolume = MasterVolume, // Füge die Lautstärke hinzu
            selectedCharacterId = _selectedCharacterId, // Speichere den ausgewählten Charakter
            unlockedMemeIds = _unlockedMemeIds.ToList() // Konvertiere HashSet zu Liste für die Serialisierung
        };

        // Wandle das Objekt in einen JSON-String um (true für "pretty print" -> lesbar)
        string json = JsonUtility.ToJson(data, true);

        // Schreibe den JSON-String in eine Datei
        File.WriteAllText(_savePath, json);

        Debug.Log($"Spielerprofil gespeichert unter: {_savePath}");
    }

    private void LoadProfile()
    {
        // Setze die Daten zurück, bevor du versuchst zu laden.
        // Das stellt sicher, dass wir einen sauberen Zustand haben, falls die Datei nicht existiert.
        this.PlayerName = null;
        this.MasterVolume = 1.0f; // Setze auf Standardwert
        this._unlockedMemeIds.Clear();
        this._selectedCharacterId = "default"; // Setze auf Standardwert

        if (File.Exists(_savePath))
        {
            // Lese den JSON-String aus der Datei
            string json = File.ReadAllText(_savePath);

            // Wandle den JSON-String zurück in ein PlayerData-Objekt
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            this._selectedCharacterId = data.selectedCharacterId; // Lade den ausgewählten Charakter
            // Lade die Daten in das aktuelle Profil
            this.PlayerName = data.playerName;
            this.MasterVolume = data.masterVolume;
            foreach (var memeId in data.unlockedMemeIds)
            {
                _unlockedMemeIds.Add(memeId);
            }
            Debug.Log($"Spielerprofil geladen von: {_savePath}");

        }
    }

    /// <summary>
    /// Löscht die Speicherdatei des Spielerprofils und setzt die aktuellen Daten im Speicher zurück.
    /// </summary>
    public void DeleteProfile()
    {
        if (File.Exists(_savePath))
        {
            File.Delete(_savePath);
            Debug.LogWarning($"Spielerprofil gelöscht von: {_savePath}");
        }

        // Setze auch die In-Memory-Daten zurück, um den Zustand sofort zu aktualisieren.
        this.PlayerName = null;
        this.MasterVolume = 1.0f;
        this._unlockedMemeIds.Clear();
        this._selectedCharacterId = "default"; // Setze auf Standardwert
    }

    /// <summary>
    /// Setzt den MasterVolume-Wert auf den AudioMixer.
    /// </summary>
    public void ApplyMasterVolume()
    {
        if (_mainAudioMixer != null)
        {
            float safeVolume = Mathf.Clamp(MasterVolume, 0.0001f, 1.0f);
            _mainAudioMixer.SetFloat("MasterVolume", Mathf.Log10(safeVolume) * 20);
            Debug.Log($"MasterVolume auf {_mainAudioMixer.name} gesetzt: {MasterVolume}");
        }
        else
        {
            Debug.LogWarning("Konnte MasterVolume nicht anwenden: Kein AudioMixer gefunden!");
        }
    }

    private void FindMainAudioMixer()
    {
        if (_mainAudioMixer != null) return;

        // Finde alle AudioMixer-Assets, die im Projekt geladen sind.
        // Diese Methode ist robust und erfordert nicht, dass der Mixer in einem "Resources"-Ordner liegt.
        var audioMixers = Resources.FindObjectsOfTypeAll<AudioMixer>();
        if (audioMixers.Length == 0)
        {
            Debug.LogError("Kein AudioMixer im Projekt gefunden!");
            return;
        }

        // Suche nach dem Mixer mit dem spezifischen Namen "MainMixer".
        foreach (var mixer in audioMixers)
        {
            if (mixer.name == "MainMixer")
            {
                _mainAudioMixer = mixer;
                break; // Wir haben ihn gefunden, die Schleife kann beendet werden.
            }
        }

        if (_mainAudioMixer == null) {
            Debug.LogError("Ein AudioMixer mit dem Namen 'MainMixer' konnte nicht gefunden werden. Bitte den Namen überprüfen!");
        }
    }
}