using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class AICommentator : MonoBehaviour
{
    [Header("UI-Elemente")]
    [Tooltip("Textfeld für die Kommentare der KI.")]
    [SerializeField] private TextMeshProUGUI commentText;
    [Tooltip("Wie lange ein Kommentar sichtbar bleibt.")]
    [SerializeField] private float commentDuration = 4.0f;

    [Header("Kommentar-Listen")]
    [SerializeField] private List<string> playerGoodMoveComments = new List<string> { "Nicht schlecht!", "Ein kluger Zug.", "Das könnte interessant werden.", "Gut gespielt." };
    [SerializeField] private List<string> playerBadMoveComments = new List<string> { "Sicher?", "Das würde ich überdenken...", "Ein mutiger, aber törichter Zug.", "Aha..." };
    [SerializeField] private List<string> aiGoodMoveComments = new List<string> { "Schachmatt in 3 Zügen.", "Genau wie geplant.", "Siehst du? Einfach.", "Ich sehe den Sieg." };
    [SerializeField] private List<string> gameStartComments = new List<string> { "Möge der Bessere gewinnen. (Also ich.)", "Auf ein gutes Spiel!", "Mal sehen, was du drauf hast." };
    [SerializeField] private List<string> playerWinningComments = new List<string> { "Glück gehabt!", "Das war knapp...", "Du treibst mich in die Enge." };
    [SerializeField] private List<string> aiWinningComments = new List<string> { "Ich hab's dir ja gesagt!", "Zu einfach.", "Nächstes Mal vielleicht." };

    private Coroutine _currentCommentCoroutine;

    private void Start()
    {
        // Verstecke den Text zu Beginn
        commentText.text = "";
    }

    public void OnGameStart()
    {
        ShowComment(gameStartComments[Random.Range(0, gameStartComments.Count)]);
    }

    public void OnPlayerWins()
    {
        ShowComment(playerWinningComments[Random.Range(0, playerWinningComments.Count)]);
    }

    public void OnAIWins()
    {
        ShowComment(aiWinningComments[Random.Range(0, aiWinningComments.Count)]);
    }

    // Diese Methode wird vom Hauptspiel aufgerufen, um einen Zug zu bewerten
    public void AnalyzePlayerMove(bool isGoodMove)
    {
        if (isGoodMove)
        {
            ShowComment(playerGoodMoveComments[Random.Range(0, playerGoodMoveComments.Count)]);
        }
        else
        {
            ShowComment(playerBadMoveComments[Random.Range(0, playerBadMoveComments.Count)]);
        }
    }
    
    public void AnalyzeAIMove()
    {
        ShowComment(aiGoodMoveComments[Random.Range(0, aiGoodMoveComments.Count)]);
    }

    private void ShowComment(string message)
    {
        if (_currentCommentCoroutine != null)
        {
            StopCoroutine(_currentCommentCoroutine);
        }
        _currentCommentCoroutine = StartCoroutine(ShowCommentCoroutine(message));
    }



    private IEnumerator ShowCommentCoroutine(string message)
    {
        commentText.text = message;
        yield return new WaitForSeconds(commentDuration);
        commentText.text = "";
    }
}
