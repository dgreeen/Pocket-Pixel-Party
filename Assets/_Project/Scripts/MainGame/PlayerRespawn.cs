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
        // Finde den offiziellen Startpunkt in der Szene.
        GameObject startPointObject = GameObject.FindGameObjectWithTag("Respawn");
        if (startPointObject != null)
        {
            _startPosition = startPointObject.transform.position;
        }
        else
        {
            Debug.LogWarning("Kein GameObject mit dem Tag 'Respawn' in der Szene gefunden. Verwende die initiale Spielerposition als Fallback.");
            _startPosition = transform.position;
        }
        _startRotation = Quaternion.identity; // Setze die Rotation auf einen Standardwert zurück.
    }

    public void Respawn()
    {
        // Setze die Position und Rotation des Spielers auf die gespeicherten Startwerte zurück.
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        Debug.Log($"Spieler wurde an Startposition {_startPosition} zurückgesetzt.");
    }
}