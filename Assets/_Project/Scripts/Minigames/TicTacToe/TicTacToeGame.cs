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

    private int[] boardState; // 0: leer, 1: Spieler (X), 2: KI (O)
    private bool isPlayerTurn;
    private bool gameOver;

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
            int index = i; // Wichtig für den Lambda-Ausdruck im Listener
            cellButtons[i].onClick.RemoveAllListeners();
            cellButtons[i].onClick.AddListener(() => OnCellClicked(index));
        }
    }

    private void OnCellClicked(int index)
    {
        if (!isPlayerTurn || gameOver || boardState[index] != 0)
        {
            return;
        }

        MakeMove(index, 1); // Spieler ist 1

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
        cellButtons[index].GetComponentInChildren<TextMeshProUGUI>().text = player == 1 ? "X" : "O";
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
        MakeMove(bestMove, 2); // KI ist 2

        if (!gameOver)
        {
            isPlayerTurn = true;
            statusText.text = "Du bist dran!";
        }
    }

    private int FindBestMove()
    {
        // 1. Kann KI gewinnen?
        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == 0)
            {
                boardState[i] = 2;
                if (CheckWin(2))
                {
                    boardState[i] = 0;
                    return i;
                }
                boardState[i] = 0;
            }
        }

        // 2. Kann Spieler gewinnen? Blockieren!
        for (int i = 0; i < 9; i++)
        {
            if (boardState[i] == 0)
            {
                boardState[i] = 1;
                if (CheckWin(1))
                {
                    boardState[i] = 0;
                    return i;
                }
                boardState[i] = 0;
            }
        }

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

        if (playerWon)
        {
            statusText.text = "Gewonnen!";
            // TODO: Belohnung freischalten
        }
        else
        {
            statusText.text = "Verloren!";
        }
        StartCoroutine(ReturnToMainGameAfterDelay());
    }

    private void EndGameDraw()
    {
        gameOver = true;
        statusText.text = "Unentschieden!";
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
            Debug.LogWarning("SceneController nicht gefunden. Rückkehr zum Hauptspiel nicht möglich.");
        }
    }
}
