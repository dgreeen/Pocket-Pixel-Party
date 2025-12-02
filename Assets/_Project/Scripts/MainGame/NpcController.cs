using UnityEngine;

/// <summary>
/// Steuert einen NPC, der zwischen zwei Punkten patrouilliert
/// und den Spieler bei Kollision zurücksetzt.
/// </summary>
public class NpcController : MonoBehaviour
{
    [Header("Bewegung")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private float speed = 2f;

    private Transform _target;

    void Start()
    {
        // NPC startet mit dem Ziel "endPoint"
        _target = endPoint;
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        // Bewege den NPC in Richtung des aktuellen Ziels
        transform.position = Vector3.MoveTowards(transform.position, _target.position, speed * Time.deltaTime);

        // Wenn das Ziel erreicht ist, wechsle das Ziel
        if (Vector3.Distance(transform.position, _target.position) < 0.1f)
        {
            if (_target == endPoint)
            {
                _target = startPoint;
            }
            else
            {
                _target = endPoint;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Prüfe, ob das kollidierte Objekt den Tag "Player" hat
        if (collision.gameObject.CompareTag("Player"))
        {
            // Hole die PlayerRespawn-Komponente und rufe die Respawn-Methode auf
            if (collision.gameObject.TryGetComponent<PlayerRespawn>(out var playerRespawn))
            {
                playerRespawn.Respawn();
            }
        }
    }
}