using UnityEngine;
using TMPro; // Wichtig für UI Text

public class PongGameManager : MonoBehaviour
{
    [Header("Spiel-Objekte")]
    public Ball ball; // Referenz auf das Ball-Skript

    [Header("Punkte")]
    public int playerScore = 0;
    public int aiScore = 0;
    public int scoreToWin = 5;

    [Header("UI-Referenzen")]
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI aiScoreText;

    [Header("Core-System")]
    // Referenz auf deinen Haupt-Controller (aus _Project/Scripts/Core)
    public SceneController sceneController; 

    void Start()
    {
        // Finde den SceneController, falls nicht zugewiesen
        if (sceneController == null)
        {
            sceneController = FindObjectOfType<SceneController>();
        }
        
        // Starte den Ball
        RestartBall();
    }

    // Wird von Goal.cs aufgerufen
    public void Score(bool isPlayerScoring)
    {
        if (isPlayerScoring)
        {
            playerScore++;
            playerScoreText.text = playerScore.ToString();
        }
        else
        {
            aiScore++;
            aiScoreText.text = aiScore.ToString();
        }

        // Prüfen, ob jemand gewonnen hat
        if (playerScore >= scoreToWin || aiScore >= scoreToWin)
        {
            EndMinigame();
        }
        else
        {
            // Ball zurücksetzen und neu starten
            ball.ResetPosition();
            Invoke(nameof(RestartBall), 1.5f); // 1.5s warten
        }
    }

    void RestartBall()
    {
        ball.Launch();
    }

    void EndMinigame()
    {
        // Stoppe den Ball
        ball.ResetPosition(); 
        
        Debug.Log("Pong Minigame beendet!");
        
        // Beispiel (musst du an deinen SceneController anpassen):
        if (sceneController != null)
        {
            sceneController.ReturnToMainGame();
        }
    }
}