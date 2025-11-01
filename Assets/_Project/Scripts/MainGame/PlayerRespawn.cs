using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Die Y-Koordinate, unter der der Spieler als 'gefallen' gilt und respawnt.")]
    public float deathZoneY = -20f;

    private Vector3 respawnPoint;
    private Rigidbody2D rb;

    void Start()
    {
        // Wir speichern die Startposition als ersten Respawn-Punkt.
        respawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Prüfen, ob der Spieler unter die Todeszone gefallen ist.
        if (transform.position.y < deathZoneY)
        {
            Respawn();
        }
    }

    void Respawn()
    {
        // Setze die Position des Spielers zurück.
        transform.position = respawnPoint;

        // Setze die Geschwindigkeit des Rigidbodys zurück, damit der Spieler nicht weiterfällt.
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        Debug.Log("Spieler ist gefallen und wurde zurückgesetzt.");
    }

    // Diese Methode kann von anderen Skripten aufgerufen werden, um den Respawn-Punkt zu aktualisieren.
    // Zum Beispiel nach dem Erreichen eines Checkpoints.
    public void SetRespawnPoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;
    }
}
