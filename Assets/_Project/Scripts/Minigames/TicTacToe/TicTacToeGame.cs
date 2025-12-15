using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;

public class TicTacToeGame : MonoBehaviour
{
    [Header("Spiel-Elemente")]
    [Tooltip("Ziehe hier alle 9 Button-Objekte der Zellen hinein.")]
    [SerializeField] private Button[] cellButtons;
    [Tooltip("Text, um den Spielstatus anzuzeigen (Sieg, Niederlage, etc.).")]
    [SerializeField] private TextMeshProUGUI statusText;
    [Header("KI-Kommentator")]
    [Tooltip("Ziehe hier den AICommentatorManager hinein.")]
    [SerializeField] private AICommentator commentator;

    private int[] boardState; // 0: leer, 1: Spieler (X), 2: KI (O)
    private bool isPlayerTurn;
    private bool gameOver;
    private const int PLAYER_ID = 1;
    private const int AI_ID = 2;

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        boardState = new int[9];
        gameOver = false;
        isPlayerTurn = true;
        statusText.text = "Du bist dran!";

        for (int i = 0; i < cellButtons.Length; i++)
        {
            cellButtons[i].interactable = true;
            cellButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "";
            int index = i; // Wichtig fuer den Lambda-Ausdruck im Listener
            cellButtons[i].onClick.RemoveAllListeners();
            cellButtons[i].onClick.AddListener(() => OnCellClicked(index));
        }

        if (commentator != null)
        {
            commentator.OnGameStart();
        }
    }

    private void OnCellClicked(int index)
    {
        if (!isPlayerTurn || gameOver || boardState[index] != 0)
        {
            return;
        }

        // Analysiere den Zug, bevor er gemacht wird
        AnalyzeAndCommentOnPlayerMove(index);

        MakeMove(index, PLAYER_ID); // Spieler ist 1

        if (!gameOver)
        {
            isPlayerTurn = false;
            statusText.text = "KI ist am Zug...";
            StartCoroutine(AITurn());
        }
    }

    private void MakeMove(int index, int player)
    {
        boardState[index] = player;
        cellButtons[index].GetComponentInChildren<TextMeshProUGUI>().text = player == PLAYER_ID ? "X" : "O";
        cellButtons[index].interactable = false;

        if (CheckWin(player))
        {
            EndGame(player == 1);
        }
        else if (boardState.All(cell => cell != 0))
        {
            EndGameDraw();
        }
    }

    private IEnumerator AITurn()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f)); // KI "denkt nach"

        int bestMove = FindBestMove();
        MakeMove(bestMove, AI_ID); // KI ist 2

        if (commentator != null && !gameOver)
        {
            commentator.AnalyzeAIMove();
        }

        if (!gameOver)
        {
            isPlayerTurn = true;
            statusText.text = "Du bist dran!";
        }
    }

    private void AnalyzeAndCommentOnPlayerMove(int moveIndex)
    {
        if (commentator == null) return;

        // Simuliert, ob der Zug "gut" war.
        // Ein "guter" Zug ist einer, der eine Gewinnchance fuer den Spieler schafft oder eine fuer die KI blockiert.
        
        // Test 1: Schafft dieser Zug eine Gewinnlinie fuer den Spieler?
        boardState[moveIndex] = PLAYER_ID;
        if (CheckWin(PLAYER_ID))
        {
            boardState[moveIndex] = 0; // Zug zuruecksetzen
            commentator.AnalyzePlayerMove(true); // Guter Zug!
            return;
        }
        boardState[moveIndex] = 0; // Zug zuruecksetzen

        // Test 2: Blockiert dieser Zug eine unmittelbare Gewinnlinie der KI?
        boardState[moveIndex] = AI_ID;
        if (CheckWin(AI_ID))
        {
            boardState[moveIndex] = 0; // Zug zuruecksetzen
            commentator.AnalyzePlayerMove(true); // Guter Zug! (Block)
            return;
        }
        boardState[moveIndex] = 0; // Zug zuruecksetzen

        // Wenn keine der beiden Bedingungen zutrifft, war es ein "normaler" oder "schlechter" Zug.
        commentator.AnalyzePlayerMove(false);
    }

    private int FindBestMove()
    {
        // 1. Kann KI gewinnen?
        int winningMove = FindWinningMove(AI_ID);
        if (winningMove != -1) return winningMove;

        // 2. Kann Spieler gewinnen? Blockieren!
        int blockingMove = FindWinningMove(PLAYER_ID);
        if (blockingMove != -1) return blockingMove;

        // 3. Nimm die Mitte, wenn frei
        if (boardState[4] == 0) return 4;

        // 4. Nimm eine freie Ecke
        int[] corners = { 0, 2, 6, 8 };
        foreach (int corner in corners.OrderBy(c => Random.value))
        {
            if (boardState[corner] == 0) return corner;
        }

        // 5. Nimm eine freie Seite
        int[] sides = { 1, 3, 5, 7 };
        foreach (int side in sides.OrderBy(s => Random.value))
        {
            if (boardState[side] == 0) return side;
        }

        return -1; // Sollte nie passieren
    }

    private int FindWinningMove(int player)
    {
        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == 0)
            {
                boardState[i] = player; // Teste den Zug
                if (CheckWin(player))
                {
                    boardState[i] = 0; // Setze das Brett zurueck
                    return i; // Gefunden!
                }
                boardState[i] = 0; // Setze das Brett zurueck
            }
        }
        return -1; // Kein direkter Gewinnzug gefunden
    }

    private bool CheckWin(int player)
    {
        int[,] winConditions = new int[,]
        {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Reihen
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Spalten
            {0, 4, 8}, {2, 4, 6}  // Diagonalen
        };

        for (int i = 0; i < winConditions.GetLength(0); i++)
        {
            if (boardState[winConditions[i, 0]] == player &&
                boardState[winConditions[i, 1]] == player &&
                boardState[winConditions[i, 2]] == player)
            {
                return true;
            }
        }
        return false;
    }

    private void EndGame(bool playerWon)
    {
        gameOver = true;
        foreach (var button in cellButtons)
        {
            button.interactable = false;
        }

        // Markiere den Versuch als beendet, damit der MemePoint verschwindet.
        if (SceneController.instance != null)
        {
            SceneController.instance.FinishCurrentMinigameAttempt();
        }

        if (playerWon)
        {
            statusText.text = "Gewonnen!";
            if (commentator != null) commentator.OnPlayerWins();
            // Sage dem SceneController, dass das Minispiel erfolgreich war.
            if (SceneController.instance != null) SceneController.instance.CompleteCurrentMinigame();
        }
        else
        {
            statusText.text = "Verloren!";
            if (commentator != null) commentator.OnAIWins();
        }

        // Die Rueckkehr zum Hauptspiel wird in beiden Faellen gestartet.
        StartCoroutine(ReturnToMainGameAfterDelay());
    }

    private void EndGameDraw()
    {
        gameOver = true;
        statusText.text = "Unentschieden!";

        // Markiere den Versuch als beendet, damit der MemePoint verschwindet.
        if (SceneController.instance != null)
        {
            SceneController.instance.FinishCurrentMinigameAttempt();
        }

        StartCoroutine(ReturnToMainGameAfterDelay());
    }

    private IEnumerator ReturnToMainGameAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (SceneController.instance != null)
        {
            SceneController.instance.ReturnToMainGame();
        }
        else
        {
            Debug.LogWarning("SceneController nicht gefunden. Rueckkehr zum Hauptspiel nicht moeglich.");
        }
    }
}
