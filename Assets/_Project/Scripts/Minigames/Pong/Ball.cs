using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public float startSpeed = 8f;
    public float speedIncreasePerHit = 0.5f;
    public float maxSpeed = 20f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch()
    {
        // Sicherheitscheck, falls Awake aus irgendeinem Grund fehlschlägt.
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // Starte in eine zufällige Richtung
        float x = Random.Range(0, 2) == 0 ? -1f : 1f;
        float y = Random.Range(0, 2) == 0 ? Random.Range(-1f, -0.5f) : Random.Range(0.5f, 1f);
        rb.velocity = new Vector2(x, y).normalized * startSpeed; // Startgeschwindigkeit setzen
    }

    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;
    }

    // Sorgt für Variation beim Abprall
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Prüfen, ob der Ball ein Paddle getroffen hat
        if (collision.transform.CompareTag("Player") || collision.transform.CompareTag("AI"))
        {
            // Geschwindigkeit erhöhen
            IncreaseSpeed();

            // Winkel anpassen, um zu flache Winkel zu vermeiden
            Vector2 velocity = rb.velocity;
            float minVerticalVelocity = rb.velocity.magnitude * 0.2f; // Mindestens 20% der Geschwindigkeit vertikal

            if (Mathf.Abs(velocity.y) < minVerticalVelocity)
            {
                velocity.y = Mathf.Sign(velocity.y) * minVerticalVelocity;
                rb.velocity = velocity;
            }
        }
    }

    private void IncreaseSpeed()
    {
        float newSpeed = Mathf.Clamp(rb.velocity.magnitude + speedIncreasePerHit, startSpeed, maxSpeed);
        rb.velocity = rb.velocity.normalized * newSpeed;
    }
}