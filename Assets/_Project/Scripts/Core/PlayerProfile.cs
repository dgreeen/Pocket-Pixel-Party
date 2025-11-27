using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; // Wichtig für Dateioperationen
using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile instance;

    public event Action<MemeData> OnMemeDataUnlocked;

    public string PlayerName { get; private set; }

    private readonly HashSet<string> _unlockedMemeIds = new HashSet<string>();

    private string _savePath;

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

        // Lade das gespeicherte Profil beim Start des Spiels
        LoadProfile();
    }

    public void SetPlayerName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) return;

        PlayerName = newName;
        // Speichere das Profil, wenn der Name geändert wird
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

    private void SaveProfile()
    {
        // Erstelle ein Datenobjekt mit den aktuellen Spielerdaten
        PlayerData data = new PlayerData
        {
            playerName = this.PlayerName,
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
        this._unlockedMemeIds.Clear();

        if (File.Exists(_savePath))
        {
            // Lese den JSON-String aus der Datei
            string json = File.ReadAllText(_savePath);

            // Wandle den JSON-String zurück in ein PlayerData-Objekt
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            // Lade die Daten in das aktuelle Profil
            this.PlayerName = data.playerName;
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
        this._unlockedMemeIds.Clear();
    }
}