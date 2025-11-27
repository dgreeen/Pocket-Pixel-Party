using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public string playerName;
    public List<string> unlockedMemeIds;

    // Konstruktor, um sicherzustellen, dass die Liste immer initialisiert ist.
    public PlayerData()
    {
        unlockedMemeIds = new List<string>();
    }
}