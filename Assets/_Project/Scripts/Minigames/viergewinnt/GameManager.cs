using UnityEngine;
using UnityEngine.UI; // Für UI-Elemente wie Text
using TMPro; // Hinzufügen für TextMeshPro
using System.Collections;

public class GameManager : MonoBehaviour
{
    private const int COLUMNS = 6;
    private const int ROWS = 6;

    // --- Variablen für das Spielbrett ---
    public GameObject playerPiecePrefab;
    public GameObject cpuPiecePrefab;
    public SpriteRenderer boardSprite; // Referenz zum Spielbrett-Sprite
    public GameObject[] spawnPoints; // Punkte über jeder Spalte, wo die Steine erscheinen

    private int[,] grid = new int[COLUMNS, ROWS]; // 6 Spalten, 6 Reihen. 0=leer, 1=Spieler, 2=CPU
    private bool isPlayerTurn = true;
    private bool gameIsOver = false;

    // --- UI-Variablen ---
    public TextMeshProUGUI statusText; // Zeigt an, wer am Zug ist oder wer gewonnen hat
    public Button restartButton;

    void Start()
    {
        restartButton.gameObject.SetActive(false);
        statusText.text = "Du bist am Zug!";
    }

    void Update()
    {
        // Nur wenn der Spieler am Zug ist und das Spiel nicht vorbei ist
        if (isPlayerTurn && !gameIsOver)
        {
            // Bei Mausklick
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int column = GetColumnFromMousePos(mousePos);

                if (column != -1)
                {
                    PlacePiece(column);
                }
            }
        }
    }

    // Findet die Spalte basierend auf der Mausposition
    private int GetColumnFromMousePos(Vector3 mousePos)
    {
        if (boardSprite == null) return -1; // Sicherheitshalber

        Bounds bounds = boardSprite.bounds;
        float boardWidth = bounds.size.x;
        float boardStartX = bounds.min.x;

        // Prüfen, ob der Klick innerhalb der horizontalen Grenzen des Bretts liegt
        if (mousePos.x > boardStartX && mousePos.x < bounds.max.x)
        {
            // Berechne die Spalte (0-6) basierend auf der Klickposition
            return Mathf.FloorToInt((mousePos.x - boardStartX) / (boardWidth / COLUMNS));
        }
        return -1; // Außerhalb des Bretts geklickt.
    }

    // Platziert einen Spielstein in der gewählten Spalte
    void PlacePiece(int column)
    {
        int row = GetNextAvailableRow(column);
        if (row == -1)
        {
            // Spalte ist voll
            Debug.Log("Diese Spalte ist voll!");
            return;
        }

        // Figur für den Spieler instanziieren
        Instantiate(playerPiecePrefab, spawnPoints[column].transform.position, Quaternion.identity);
        grid[column, row] = 1; // Spieler-ID im Grid speichern

        // Prüfen, ob der Spieler gewonnen hat
        if (CheckForWin(1, column, row))
        {
            EndGame("Du hast gewonnen!");
            return;
        }
        
        // Prüfen auf Unentschieden
        if (CheckForDraw())
        {
            EndGame("Unentschieden!");
            return;
        }

        // Zug wechseln
        isPlayerTurn = false;
        statusText.text = "Gegner ist am Zug";

        // CPU-Zug mit einer kleinen Verzögerung starten, um die Illusion des Nachdenkens zu erzeugen
        StartCoroutine(CPUTurn());
    }

    // Findet die nächste freie Reihe in einer Spalte
    private int GetNextAvailableRow(int column)
    {
        for (int i = 0; i < ROWS; i++)
        {
            if (grid[column, i] == 0)
            {
                return i;
            }
        }
        return -1; // Spalte ist voll
    }

    // Die Logik für den CPU-Zug
    IEnumerator CPUTurn()
    {
        yield return new WaitForSeconds(1.5f); // Kurze Pause, um den Zug zu simulieren

        // Die neue, schlauere Logik, um die beste Spalte zu finden
        int column = ChooseBestColumn();
        int row = GetNextAvailableRow(column);
        if (row == -1) yield break; // Sollte nicht passieren, aber sicher ist sicher

        // Figur für die CPU instanziieren
        Instantiate(cpuPiecePrefab, spawnPoints[column].transform.position, Quaternion.identity);
        grid[column, row] = 2; // CPU-ID im Grid speichern

        // Prüfen, ob die CPU gewonnen hat
        if (CheckForWin(2, column, row))
        {
            EndGame("CPU hat gewonnen!");
            yield break;
        }
        
        // Prüfen auf Unentschieden
        if (CheckForDraw())
        {
            EndGame("Unentschieden!");
            yield break;
        }

        // Zug zurück zum Spieler wechseln
        isPlayerTurn = true;
        statusText.text = "Du bist am Zug!";
    }

    private int ChooseBestColumn()
    {
        // Priorität 1: Kann die CPU gewinnen?
        for (int c = 0; c < COLUMNS; c++)
        {
            int r = GetNextAvailableRow(c);
            if (r != -1)
            {
                grid[c, r] = 2; // Simuliere CPU-Zug
                if (CheckForWin(2, c, r))
                {
                    grid[c, r] = 0; // Simulation zurücksetzen
                    return c; // Diesen Gewinnzug ausführen!
                }
                grid[c, r] = 0; // Simulation zurücksetzen
            }
        }

        // Priorität 2: Muss die CPU den Spieler blockieren?
        for (int c = 0; c < COLUMNS; c++)
        {
            int r = GetNextAvailableRow(c);
            if (r != -1)
            {
                grid[c, r] = 1; // Simuliere Spieler-Zug
                if (CheckForWin(1, c, r))
                {
                    grid[c, r] = 0; // Simulation zurücksetzen
                    return c; // Diesen Block-Zug ausführen!
                }
                grid[c, r] = 0; // Simulation zurücksetzen
            }
        }

        // Priorität 3: Wähle eine zufällige, gültige Spalte
        System.Collections.Generic.List<int> availableColumns = new System.Collections.Generic.List<int>();
        for (int c = 0; c < COLUMNS; c++)
        {
            if (GetNextAvailableRow(c) != -1)
            {
                availableColumns.Add(c);
            }
        }
        
        // Wenn es verfügbare Spalten gibt, wähle eine zufällige davon aus.
        if (availableColumns.Count > 0)
        {
            int randomIndex = Random.Range(0, availableColumns.Count);
            return availableColumns[randomIndex];
        }

        return 0; // Fallback, sollte nur bei einem vollen Brett erreicht werden.
    }


    // Beendet das Spiel
    void EndGame(string message)
    {
        gameIsOver = true;
        statusText.text = message;
        restartButton.gameObject.SetActive(true);
    }
    
    // Startet das Spiel neu (wird vom Button aufgerufen)
    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // Prüft, ob das Brett voll ist
    bool CheckForDraw()
    {
        for (int x = 0; x < COLUMNS; x++)
        {
            if (grid[x, ROWS - 1] == 0) // Nur die oberste Reihe jeder Spalte prüfen
            {
                return false; // Wenn eine Spalte oben noch frei ist, ist es kein Unentschieden
            }
        }
        return true; // Alle Spalten sind voll
    }

    // Überprüft, ob der letzte Zug zu einem Sieg geführt hat
    bool CheckForWin(int playerID, int x, int y)
    {
        // Horizontal prüfen
        int count = 0;
        for (int i = 0; i < COLUMNS; i++)
        {
            count = (grid[i, y] == playerID) ? count + 1 : 0;
            if (count >= 4) return true;
        }

        // Vertikal prüfen
        count = 0;
        for (int i = 0; i < ROWS; i++)
        {
            count = (grid[x, i] == playerID) ? count + 1 : 0;
            if (count >= 4) return true;
        }

        // Diagonal prüfen (unten links nach oben rechts)
        count = 0;
        for (int i = -3; i <= 3; i++)
        {
            int checkX = x + i;
            int checkY = y + i;
            if (checkX >= 0 && checkX < COLUMNS && checkY >= 0 && checkY < ROWS)
            {
                count = (grid[checkX, checkY] == playerID) ? count + 1 : 0;
                if (count >= 4) return true;
            }
        }

        // Diagonal (oben links nach unten rechts)
        count = 0;
        for (int i = -3; i <= 3; i++)
        {
            int checkX = x + i;
            int checkY = y - i;
            if (checkX >= 0 && checkX < COLUMNS && checkY >= 0 && checkY < ROWS)
            {
                count = (grid[checkX, checkY] == playerID) ? count + 1 : 0;
                if (count >= 4) return true;
            }
        }

        return false;
    }
}
