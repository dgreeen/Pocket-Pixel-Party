using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIPaddle : MonoBehaviour
{
    [Header("Bewegung")]
    public float speed = 6f;
    private Rigidbody2D rb;

    [Header("Referenzen")]
    public Transform ball; // Hier ziehen wir den Ball rein

    [Header("Grenzen")]
    public Transform topWall;
    public Transform bottomWall;

    private float paddleHeight;
    private float topBoundary;
    private float bottomBoundary;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Berechne die Höhe des Schlägers zur Hälfte
        paddleHeight = transform.localScale.y / 2;

        // Berechne die Bewegungsgrenzen basierend auf den Wänden
        topBoundary = topWall.position.y - paddleHeight;
        bottomBoundary = bottomWall.position.y + paddleHeight;
    }

    void FixedUpdate()
    {
        // Prüfen, ob eine Referenz zum Ball existiert
        if (ball != null)
        {
            // Bestimme die Richtung zum Ball
            float targetY = ball.position.y;
            
            // Bewege den Schläger in Richtung der Y-Position des Balls
            Vector2 currentPosition = rb.position;
            Vector2 targetPosition = new Vector2(currentPosition.x, targetY);
            
            // Bewege den Schläger mit einer maximalen Geschwindigkeit
            Vector2 newPosition = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.fixedDeltaTime);

            // Klemme die Y-Position fest, damit er nicht aus den Grenzen kommt
            float clampedY = Mathf.Clamp(newPosition.y, bottomBoundary, topBoundary);
            newPosition.y = clampedY; // Wende das Clamping an
        
            // Bewege den Schläger an die neue Position
            rb.MovePosition(newPosition);
        }
    }
}