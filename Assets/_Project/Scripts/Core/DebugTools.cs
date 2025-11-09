using UnityEngine;

public class DebugTools : MonoBehaviour
{
    [ContextMenu("!!! DELETE ALL PLAYERPREFS !!!")]
    void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        Debug.LogWarning("Alle PlayerPrefs wurden gelöscht!");
    }
}
