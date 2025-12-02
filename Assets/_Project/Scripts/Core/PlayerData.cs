using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string playerName;
    public float masterVolume; // Hinzugefügt: Feld für die Master-Lautstärke
    public string selectedCharacterId;
    public List<string> unlockedMemeIds;

    // Konstruktor, um sicherzustellen, dass die Liste immer initialisiert ist.
    public PlayerData()
    {
        unlockedMemeIds = new List<string>();
        masterVolume = 1.0f; // Standardwert für die Lautstärke
        selectedCharacterId = "default"; // Standardwert, falls nichts ausgewählt ist
    }
}