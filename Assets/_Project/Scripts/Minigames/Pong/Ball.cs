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
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("AI"))
        {
            // Wir lassen die Physik-Engine den Abprall berechnen.
            // Unser Job ist es nur, die Geschwindigkeit nach dem Abprall zu erhöhen.
            // Wir machen das im nächsten Frame, um sicherzustellen, dass die Physik-Kalkulation abgeschlossen ist.
            Invoke(nameof(IncreaseSpeedAfterBounce), 0f);
        }
    }

    private void IncreaseSpeedAfterBounce()
    {
        // Nimm die aktuelle Geschwindigkeit (nach dem Abprall) und erhöhe sie.
        float currentSpeed = rb.velocity.magnitude;
        float newSpeed = Mathf.Clamp(currentSpeed + speedIncreasePerHit, startSpeed, maxSpeed);
        rb.velocity = rb.velocity.normalized * newSpeed;
    }
}