using UnityEngine;
using System.Collections; // Wichtig für Coroutinen
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
    public TextMeshProUGUI countdownText; // Hier das neue Text-Objekt reinziehen

    [Header("Core-System")]
    // Referenz auf deinen Haupt-Controller (aus _Project/Scripts/Core)
    public SceneController sceneController; 

    void Start()
    {
        // Finde den SceneController, falls nicht zugewiesen
        if (sceneController == null)
        {
            sceneController = FindFirstObjectByType<SceneController>();
        }
        
        // Sicherheitscheck, um NullReferenceExceptions zu vermeiden
        if (countdownText == null)
        {
            Debug.LogError("Fehler: Die 'Countdown Text'-Referenz im PongGameManager ist nicht zugewiesen. Bitte ziehe das UI-Text-Objekt in den Inspector.");
            return; // Verhindert, dass der Code weiterläuft und einen Fehler verursacht
        }

        // Starte die erste Runde mit einem Countdown
        StartCoroutine(StartRound());
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
            StartCoroutine(StartRound());
        }
    }

    IEnumerator StartRound()
    {
        // Ball zurücksetzen und sicherstellen, dass er stillsteht
        ball.ResetPosition();

        // Countdown-Text aktivieren
        countdownText.gameObject.SetActive(true);

        // Countdown herunterzählen
        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        // Countdown-Text ausblenden und Ball starten
        countdownText.gameObject.SetActive(false);
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