using UnityEngine;

/// <summary>
/// Verwaltet die Respawn-Logik des Spielers. Setzt den Spieler immer zum Level-Anfang zurück.
/// </summary>
public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn-Punkte")]
    [Tooltip("Der Startpunkt des Levels. Wenn leer, wird nach dem 'Respawn'-Tag gesucht.")]
    [SerializeField] private Transform levelStartPoint;

    private Vector3 _initialPosition;
    private Rigidbody2D _rb;

    void Awake()
    {
        // Speichere die Startposition als ultimativen Fallback.
        _initialPosition = transform.position;
        _rb = GetComponent<Rigidbody2D>();

        // 1. Prüfe, ob ein Startpunkt direkt zugewiesen wurde.
        if (levelStartPoint != null)
        {
            Debug.Log($"PlayerRespawn nutzt zugewiesenen Startpunkt: {levelStartPoint.name}");
            return; // Erfolg, keine weitere Suche nötig.
        }

        // 2. Wenn nicht, suche nach einem Objekt mit dem "Respawn"-Tag.
        GameObject respawnObject = GameObject.FindGameObjectWithTag("Respawn");
        if (respawnObject != null)
        {
            levelStartPoint = respawnObject.transform;
            Debug.Log($"PlayerRespawn hat Startpunkt via Tag gefunden: {levelStartPoint.name}");
        }
        else // 3. Wenn beides fehlschlägt, gib eine Warnung aus.
        {
            Debug.LogWarning("Kein Level-Startpunkt zugewiesen und kein GameObject mit Tag 'Respawn' gefunden. Nutze die initiale Spielerposition als Fallback.");
        }
    }

    /// <summary>
    /// Setzt den Spieler an den Anfang des Levels zurück.
    /// </summary>
    public void Respawn()
    {
        // Bestimme die Zielposition: der zugewiesene Startpunkt oder die initiale Position als Fallback.
        Vector3 targetPosition = levelStartPoint != null ? levelStartPoint.position : _initialPosition;

        // Setze die Position und stoppe jegliche Bewegung.
        transform.position = targetPosition;
        if (_rb != null)
        {
            _rb.velocity = Vector2.zero;
        }

        Debug.Log($"Spieler an Startpunkt {targetPosition} zurückgesetzt.");
    }
}