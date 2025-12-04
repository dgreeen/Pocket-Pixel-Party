using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prüfe, ob das Objekt, das den Trigger betreten hat, der Spieler ist.
        if (other.CompareTag("Player"))
        {
            // Versuche, die PlayerRespawn-Komponente zu bekommen und die Respawn-Methode aufzurufen.
            if (other.TryGetComponent<PlayerRespawn>(out var playerRespawn))
            {
                playerRespawn.Respawn();
            }
        }
    }
}