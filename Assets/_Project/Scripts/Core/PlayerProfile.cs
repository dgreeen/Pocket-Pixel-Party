using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile instance;

    public event Action<MemeData> OnMemeDataUnlocked;

    public string PlayerName { get; private set; }

    private readonly HashSet<string> _unlockedMemeIds = new HashSet<string>();
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

        // Standard-Namen festlegen, falls keiner eingegeben wird.
        PlayerName = "Spieler";
    }

    public void SetPlayerName(string newName)
    {
        PlayerName = newName;
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
            return true;
        }
        return false;
    }

    public bool HasUnlockedMeme(string memeId) {
        return _unlockedMemeIds.Contains(memeId);
    }
}