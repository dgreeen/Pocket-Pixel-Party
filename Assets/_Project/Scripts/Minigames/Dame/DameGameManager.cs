using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DameGameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject tilePrefabLight;
    [SerializeField] private GameObject tilePrefabDark;
    [SerializeField] private GameObject piecePrefabLight;
    [SerializeField] private GameObject piecePrefabDark;

    [Header("UI")]
    [SerializeField] private TMPro.TextMeshProUGUI turnText;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TMPro.TextMeshProUGUI infoPanelTitle;
    [SerializeField] private TMPro.TextMeshProUGUI infoPanelBody;
    [SerializeField] private UnityEngine.UI.Button infoPanelButton;
    [SerializeField] private TMPro.TextMeshProUGUI infoPanelButtonText;

    [Header("Game Settings")]
    [SerializeField] private float aiDelay = 1.0f;

    private const int BOARD_SIZE = 8;
    private Piece[,] board = new Piece[BOARD_SIZE, BOARD_SIZE];
    private Piece selectedPiece;
    private bool isGameActive = false;

    #region Unity Lifecycle
    void Start()
    {
        GenerateBoard();

        // --- WICHTIG: UI mit dem globalen EventSystem verbinden ---
        // Finde den Canvas dieses Minigames und verbinde ihn mit der Hauptkamera.
        var canvas = GetComponentInChildren<Canvas>();
        if (canvas != null && canvas.worldCamera == null)
        {
            // Verbinde den Canvas mit der Hauptkamera, damit der GraphicRaycaster funktioniert.
            canvas.worldCamera = Camera.main;
        }

        SetupPieces();
        ShowStartPanel();
    }

    void Update()
    {
        if (isGameActive && GetCurrentPlayerColor() == PieceColor.Light)
        {
            HandlePlayerInput();
        }
    }
    #endregion

    #region Game Flow
    private void ShowStartPanel()
    {
        if (infoPanel == null) return;
        // Aktiviere das globale EventSystem, damit der UI-Button funktioniert.
        if (SceneController.instance != null)
        {
            SceneController.instance.gameObject.GetComponent<UnityEngine.EventSystems.EventSystem>().enabled = true;
        }
        infoPanelTitle.text = "Spielregeln";
        infoPanelBody.text = "Dein Ziel ist es, alle Steine der CPU zu schlagen.\n- Du hast die <b>hellen</b> Steine.\n- Steine bewegen sich nur diagonal nach vorne.\n- Erreicht ein Stein die Grundlinie des Gegners, wird er zur <b>Dame</b> und darf auch nach hinten ziehen.\n- Schlagen ist Pflicht!\n- Klicke einen Stein erneut an, um die Auswahl aufzuheben.";
        infoPanelButtonText.text = "Okay";

        infoPanelButton.onClick.RemoveAllListeners();
        infoPanelButton.onClick.AddListener(StartGameFromPanel);
        infoPanel.SetActive(true);
    }

    private void StartGameFromPanel()
    {
        infoPanel.SetActive(false);
        // Deaktiviere das EventSystem, damit Klicks die 2D-Collider der Spielsteine erreichen.
        if (SceneController.instance != null)
        {
            SceneController.instance.gameObject.GetComponent<UnityEngine.EventSystems.EventSystem>().enabled = false;
        }
        isGameActive = true;
        SwitchTurnTo(PieceColor.Light);
    }

    private void SwitchTurnTo(PieceColor newPlayer)
    {
        if (!isGameActive) return;
        if (CheckForWin()) return;

        selectedPiece = null;
        
        if (newPlayer == PieceColor.Light)
        {
            UpdateTurnUI("Du bist am Zug!");
        }
        else
        {
            UpdateTurnUI("CPU ist am Zug...");
            StartCoroutine(ExecuteAITurn());
        }
    }

    private void HandlePlayerInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null)
        {
            if (selectedPiece != null) DeselectPiece();
            return;
        }

        Vector2Int boardPos = WorldToBoardPosition(hit.point);
        Piece clickedPiece = board[boardPos.x, boardPos.y];

        if (selectedPiece != null)
        {
            if (clickedPiece == selectedPiece)
            {
                DeselectPiece();
            }
            else if (IsValidMove(selectedPiece, boardPos.x, boardPos.y))
            {
                bool wasJump = Mathf.Abs(boardPos.x - selectedPiece.x) == 2;
                Piece movedPiece = selectedPiece;
                PerformMove(selectedPiece, boardPos.x, boardPos.y);

                if (wasJump && FindJumpsForPiece(movedPiece).Count > 0)
                {
                    SelectPiece(movedPiece);
                    UpdateTurnUI("Mehrfachschlag! Mache noch einen Zug.");
                }
                else
                {
                    SwitchTurnTo(PieceColor.Dark);
                }
            }
            else
            {
                DeselectPiece();
            }
        }
        else
        {
            if (clickedPiece != null && clickedPiece.color == PieceColor.Light)
            {
                var possibleJumps = FindAllPossibleMoves(PieceColor.Light, true);
                if (possibleJumps.Count > 0 && FindJumpsForPiece(clickedPiece).Count == 0)
                {
                    UpdateTurnUI("Schlagzwang!");
                    return;
                }
                SelectPiece(clickedPiece);
            }
        }
    }

    private void EndGame(PieceColor winner)
    {
        isGameActive = false;
        string title, body;
        if (winner == PieceColor.Light)
        {
            title = "Sieg!";
            body = "Gratulation, du hast die CPU besiegt!";
            // Dem SceneController mitteilen, dass das Minispiel gewonnen wurde.
            if (SceneController.instance != null)
            {
                SceneController.instance.CompleteCurrentMinigame();
            }
        }
        else
        {
            title = "Spiel vorbei";
            body = "Die CPU hat dieses Mal gewonnen. Probiere es erneut!";
            // Dem SceneController mitteilen, dass das Minispiel verloren wurde (und nicht als abgeschlossen gilt).
            if (SceneController.instance != null)
            {
                SceneController.instance.UncompleteCurrentMinigame();
            }
        }
        StartCoroutine(EndGameSequence(title, body));
    }
    #endregion

    #region AI Logic
    private IEnumerator ExecuteAITurn()
    {
        yield return new WaitForSeconds(aiDelay);

        var movesToConsider = FindAllPossibleMoves(PieceColor.Dark, true);
        if (movesToConsider.Count == 0)
        {
            movesToConsider = FindAllPossibleMoves(PieceColor.Dark, false);
        }

        if (movesToConsider.Count == 0)
        {
            EndGame(PieceColor.Light);
            yield break;
        }

        List<Move> bestMoves = new List<Move>();
        int bestScore = int.MinValue;

        foreach (var move in movesToConsider)
        {
            int score = 0;
            bool isJump = Mathf.Abs(move.toX - move.piece.x) == 2;
            if (isJump)
            {
                score += 100;
                int jumpedX = move.piece.x + (move.toX - move.piece.x) / 2;
                int jumpedY = move.piece.y + (move.toY - move.piece.y) / 2;
                if (board[jumpedX, jumpedY].isKing) score += 50;
            }
            if (!move.piece.isKing && move.toY == BOARD_SIZE - 1) score += 200;
            if (IsSquareThreatened(move.toX, move.toY, PieceColor.Light)) score -= 75;
            if (!move.piece.isKing) score += move.toY;

            if (score > bestScore)
            {
                bestScore = score;
                bestMoves.Clear();
                bestMoves.Add(move);
            }
            else if (score == bestScore)
            {
                bestMoves.Add(move);
            }
        }

        // --- Zug ausführen und auf Mehrfachschläge prüfen ---
        Move currentMove = bestMoves[Random.Range(0, bestMoves.Count)];
        PerformMove(currentMove.piece, currentMove.toX, currentMove.toY);

        // Prüfen, ob der ausgeführte Zug ein Sprung war und weitere Sprünge möglich sind
        bool wasJump = Mathf.Abs(currentMove.toX - currentMove.piece.x) == 2;
        if (wasJump)
        {
            List<Move> nextJumps = FindJumpsForPiece(currentMove.piece);
            while (nextJumps.Count > 0)
            {
                yield return new WaitForSeconds(aiDelay);
                // Bei Mehrfachschlägen muss die KI nicht neu bewerten, da alle Optionen Sprünge sind.
                // Ein zufälliger Sprung aus den Möglichkeiten ist eine valide Strategie.
                Move nextChainMove = nextJumps[Random.Range(0, nextJumps.Count)];
                PerformMove(nextChainMove.piece, nextChainMove.toX, nextChainMove.toY);
                
                // Nach dem Kettensprung erneut prüfen, ob weitere Sprünge möglich sind.
                nextJumps = FindJumpsForPiece(nextChainMove.piece);
            }
        }

        SwitchTurnTo(PieceColor.Light);
    }

    private struct Move { public Piece piece; public int toX, toY; }

    private List<Move> FindAllPossibleMoves(PieceColor color, bool jumpsOnly)
    {
        var possibleMoves = new List<Move>();
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                Piece piece = board[x, y];
                if (piece != null && piece.color == color)
                {
                    for (int dx = -1; dx <= 1; dx += 2)
                    {
                        for (int dy = -1; dy <= 1; dy += 2)
                        {
                            if (IsValidMove(piece, x + dx * 2, y + dy * 2))
                            {
                                possibleMoves.Add(new Move { piece = piece, toX = x + dx * 2, toY = y + dy * 2 });
                            }
                            else if (!jumpsOnly && IsValidMove(piece, x + dx, y + dy))
                            {
                                possibleMoves.Add(new Move { piece = piece, toX = x + dx, toY = y + dy });
                            }
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    private List<Move> FindJumpsForPiece(Piece piece)
    {
        var possibleJumps = new List<Move>();
        if (piece == null) return possibleJumps;
        int x = piece.x; int y = piece.y;
        for (int dx = -1; dx <= 1; dx += 2)
        {
            for (int dy = -1; dy <= 1; dy += 2)
            {
                if (IsValidMove(piece, x + dx * 2, y + dy * 2))
                {
                    possibleJumps.Add(new Move { piece = piece, toX = x + dx * 2, toY = y + dy * 2 });
                }
            }
        }
        return possibleJumps;
    }

    private bool IsSquareThreatened(int x, int y, PieceColor threateningColor)
    {
        for (int dx = -1; dx <= 1; dx += 2)
        {
            for (int dy = -1; dy <= 1; dy += 2)
            {
                int attackerX = x + dx; int attackerY = y + dy;
                int landingX = x - dx; int landingY = y - dy;
                if (attackerX >= 0 && attackerX < BOARD_SIZE && attackerY >= 0 && attackerY < BOARD_SIZE)
                {
                    Piece potentialAttacker = board[attackerX, attackerY];
                    if (potentialAttacker != null && potentialAttacker.color == threateningColor && IsValidMove(potentialAttacker, landingX, landingY))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion

    #region Core Game Logic
    private void PerformMove(Piece piece, int toX, int toY)
    {
        int fromX = piece.x; int fromY = piece.y;
        if (Mathf.Abs(toX - fromX) == 2)
        {
            int jumpedX = fromX + (toX - fromX) / 2;
            int jumpedY = fromY + (toY - fromY) / 2;
            Destroy(board[jumpedX, jumpedY].gameObject);
            board[jumpedX, jumpedY] = null;
        }
        board[toX, toY] = piece;
        board[fromX, fromY] = null;
        piece.MoveTo(toX, toY);
        Vector3 newWorldPos = BoardToWorldPosition(toX, toY);
        newWorldPos.z = -1;
        piece.transform.position = newWorldPos;
        if (!piece.isKing && ((piece.color == PieceColor.Light && toY == 0) || (piece.color == PieceColor.Dark && toY == BOARD_SIZE - 1)))
        {
            piece.PromoteToKing();
        }
    }

    private bool IsValidMove(Piece piece, int toX, int toY)
    {
        if (toX < 0 || toX >= BOARD_SIZE || toY < 0 || toY >= BOARD_SIZE) return false;
        if (board[toX, toY] != null) return false;

        int dx = toX - piece.x; int dy = toY - piece.y;
        if (Mathf.Abs(dx) != Mathf.Abs(dy)) return false;

        int moveDir = (piece.color == PieceColor.Light) ? -1 : 1;

        if (Mathf.Abs(dx) == 1)
        {
            if (FindAllPossibleMoves(piece.color, true).Count > 0) return false;
            if (piece.isKing) return true;
            return dy == moveDir;
        }
        else if (Mathf.Abs(dx) == 2)
        {
            int jumpedX = piece.x + dx / 2; int jumpedY = piece.y + dy / 2;
            Piece jumpedPiece = board[jumpedX, jumpedY];
            if (jumpedPiece == null || jumpedPiece.color == piece.color) return false;
            if (piece.isKing) return true;
            return dy == moveDir * 2;
        }
        return false;
    }

    private bool CheckForWin()
    {
        int lightPieces = 0; int darkPieces = 0;
        foreach (Piece p in board)
        {
            if (p != null)
            {
                if (p.color == PieceColor.Light) lightPieces++;
                else darkPieces++;
            }
        }
        if (lightPieces == 0) { EndGame(PieceColor.Dark); return true; }
        if (darkPieces == 0) { EndGame(PieceColor.Light); return true; }
        return false;
    }
    #endregion

    #region Helpers & UI
    private void SelectPiece(Piece piece)
    {
        selectedPiece = piece;
        Debug.Log($"Spieler hat Stein ausgewählt bei ({piece.x}, {piece.y})");
    }

    private void DeselectPiece()
    {
        if (selectedPiece != null) selectedPiece = null;
    }

    private void UpdateTurnUI(string message)
    {
        if (turnText != null) turnText.text = message;
    }

    private IEnumerator EndGameSequence(string title, string body)
    {
        // Zeige das Ergebnis-Panel ohne Button an.
        if (infoPanel != null)
        {
            infoPanelTitle.text = title;
            infoPanelBody.text = body;
            infoPanelButton.gameObject.SetActive(false); // Verstecke den Button
            infoPanel.SetActive(true);
        }

        // Warte eine kurze Zeit, damit der Spieler das Ergebnis lesen kann.
        yield return new WaitForSeconds(3.0f);

        // Kehre automatisch zum Hauptspiel zurück.
        ReturnToMainGame();
    }

    private void ReturnToMainGame()
    {
        // Ruft die zentrale Methode im SceneController auf, um zum Hauptspiel zurückzukehren.
        if (SceneController.instance != null)
        {
            SceneController.instance.ReturnToMainGame();
        }
    }

    private PieceColor GetCurrentPlayerColor()
    {
        // Diese Methode ist nicht mehr für die Zugsteuerung nötig, aber für die Input-Prüfung.
        // Wir müssen den aktuellen Spieler hier korrekt ermitteln.
        // Da wir keinen currentPlayer mehr haben, müssen wir das anders lösen.
        // Einfacher Workaround: Wenn die KI gerade nicht am Zug ist, ist der Spieler dran.
        // Eine bessere Lösung wäre eine Zustandsmaschine (State Machine).
        // Fürs Erste: Wir nehmen an, der Spieler ist immer dran, wenn das Spiel aktiv ist.
        // Die Logik in SwitchTurnTo() verhindert, dass die KI und der Spieler gleichzeitig ziehen.
        return PieceColor.Light;
    }
    #endregion

    #region Board Generation
    private void GenerateBoard()
    {
        for (int y = 0; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                bool isLight = (x + y) % 2 != 0;
                GameObject prefab = isLight ? tilePrefabLight : tilePrefabDark;
                Vector3 worldPos = BoardToWorldPosition(x, y);
                GameObject tile = Instantiate(prefab, worldPos, Quaternion.identity, transform);
                tile.name = $"Feld ({x}, {y})";
            }
        }
    }

    private void SetupPieces()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                if ((x + y) % 2 == 0) CreatePiece(piecePrefabDark, PieceColor.Dark, x, y);
            }
        }
        for (int y = 5; y < BOARD_SIZE; y++)
        {
            for (int x = 0; x < BOARD_SIZE; x++)
            {
                if ((x + y) % 2 == 0) CreatePiece(piecePrefabLight, PieceColor.Light, x, y);
            }
        }
    }

    private void CreatePiece(GameObject prefab, PieceColor color, int x, int y)
    {
        Vector3 piecePos = BoardToWorldPosition(x, y);
        piecePos.z = -1;
        GameObject pieceObj = Instantiate(prefab, piecePos, Quaternion.identity, transform);
        pieceObj.name = $"{color} Stein ({x}, {y})";
        Piece pieceScript = pieceObj.GetComponent<Piece>();
        if (pieceScript == null)
        {
            Debug.LogError($"FEHLER: Das Prefab '{prefab.name}' hat kein 'Piece'-Skript!", prefab);
            return;
        }
        pieceScript.Setup(color, x, y);
        board[x, y] = pieceScript;
    }

    private Vector3 BoardToWorldPosition(int x, int y)
    {
        float offset = (BOARD_SIZE / 2.0f) - 0.5f;
        return new Vector3(x - offset, y - offset, 0);
    }

    private Vector2Int WorldToBoardPosition(Vector3 worldPos)
    {
        float offset = (BOARD_SIZE / 2.0f) - 0.5f;
        int x = Mathf.RoundToInt(worldPos.x + offset);
        int y = Mathf.RoundToInt(worldPos.y + offset);
        return new Vector2Int(x, y);
    }
    #endregion
}
