using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile instance;

    public event Action<MemeData> OnMemeDataUnlocked;

    public string PlayerName { get; private set; }

    private readonly HashSet<string> _unlockedMemeIds = new HashSet<string>();

    // Schlüssel für das Speichern der Daten
    private const string PlayerNameKey = "PlayerName";
    private const string UnlockedMemesKey = "UnlockedMemes";
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
        // Speichere den Spielernamen
        PlayerPrefs.SetString(PlayerNameKey, PlayerName);

        // Wandle das HashSet der Meme-IDs in einen einzigen String um (getrennt durch Kommas)
        string memeIdsString = string.Join(",", _unlockedMemeIds);
        PlayerPrefs.SetString(UnlockedMemesKey, memeIdsString);

        // Schreibe die Daten auf die Festplatte
        PlayerPrefs.Save();
        Debug.Log("Spielerprofil gespeichert!");
    }

    private void LoadProfile()
    {
        // Lade den Spielernamen, mit "Spieler" als Standardwert, falls nichts gespeichert ist
        PlayerName = PlayerPrefs.GetString(PlayerNameKey, "Spieler");

        // Lade den String der Meme-IDs
        string memeIdsString = PlayerPrefs.GetString(UnlockedMemesKey, "");
        if (!string.IsNullOrEmpty(memeIdsString))
        {
            // Lösche alte Einträge, bevor neue geladen werden
            _unlockedMemeIds.Clear();
            // Spalte den String wieder in einzelne IDs auf und füge sie dem HashSet hinzu
            List<string> memeIds = memeIdsString.Split(',').ToList();
            foreach (var id in memeIds) {
                _unlockedMemeIds.Add(id);
            }
        }
        Debug.Log("Spielerprofil geladen!");
    }
}