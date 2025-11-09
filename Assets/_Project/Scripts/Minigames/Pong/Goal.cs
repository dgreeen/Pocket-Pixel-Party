using UnityEngine;

public class Goal : MonoBehaviour
{
    // Dies setzen wir im Inspector:
    // Für RightGoal (wo der Spieler trifft) = true
    // Für LeftGoal (wo die KI trifft) = false
    public bool isPlayerGoal; 
    
    private PongGameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<PongGameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Prüfen, ob es der Ball war
        if (collision.gameObject.GetComponent<Ball>() != null)
        {
            if (isPlayerGoal)
            {
                // KI hat getroffen (weil es das rechte Tor ist)
                gameManager.Score(false); 
            }
            else
            {
                // Spieler hat getroffen (weil es das linke Tor ist)
                gameManager.Score(true);
            }
        }
    }
}