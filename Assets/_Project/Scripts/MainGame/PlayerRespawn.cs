using UnityEngine;

/// <summary>
/// Dieses Skript merkt sich die Startposition des Spielers
/// und bietet eine Methode zum Zurücksetzen an.
/// </summary>
public class PlayerRespawn : MonoBehaviour
{
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    void Awake()
    {
        // Speichere die initiale Position und Rotation, wenn der Spieler erstellt wird.
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    public void Respawn()
    {
        // Setze die Position und Rotation des Spielers auf die gespeicherten Startwerte zurück.
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        Debug.Log($"Spieler wurde an Startposition {_startPosition} zurückgesetzt.");
    }
}