using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Pruefe, ob das Objekt, das den Trigger betreten hat, der Spieler ist.
        if (other.CompareTag("Player"))
        {
            // Versuche, die PlayerRespawn-Komponente zu bekommen und die Respawn-Methode aufzurufen.
            if (other.TryGetComponent<PlayerRespawn>(out var playerRespawn))
            {
                // Setze den Spieler an den Anfang des Levels zurueck.
                playerRespawn.Respawn();
            }
        }
    }
}