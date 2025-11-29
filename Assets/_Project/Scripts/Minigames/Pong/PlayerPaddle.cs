using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerPaddleController : MonoBehaviour
{
    [Header("Bewegung")]
    public float speed = 10f;
    private Rigidbody2D rb;

    [Header("Grenzen")]
    public Transform topWall;
    public Transform bottomWall;
    public float boundaryOffset = 0.1f; // Kleiner Puffer zur Wand

    private float paddleHeight;
    private float topBoundary;
    private float bottomBoundary;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Berechne die Höhe des Schlägers zur Hälfte
        paddleHeight = transform.localScale.y / 2;

        // Berechne die Bewegungsgrenzen basierend auf den Wänden
        topBoundary = topWall.position.y - paddleHeight - boundaryOffset;
        bottomBoundary = bottomWall.position.y + paddleHeight + boundaryOffset;
    }

    void FixedUpdate()
    {
        // Lese die vertikale Eingabe (Pfeiltasten hoch/runter oder W/S)
        float moveInput = Input.GetAxis("Vertical");
    
        // Berechne die neue Position
        Vector2 targetPosition = rb.position + new Vector2(0, moveInput * speed * Time.fixedDeltaTime);
        
        // Klemme die Y-Position fest, damit er nicht aus den Grenzen kommt
        float clampedY = Mathf.Clamp(targetPosition.y, bottomBoundary, topBoundary);
        targetPosition.y = clampedY; // Wende das Clamping an
        
        // Bewege den Schläger an die neue Position
        rb.MovePosition(targetPosition);
    }
}
