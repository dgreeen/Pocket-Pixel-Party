// Assets/_Projects/Scripts/Minigames/Checkers/Piece.cs
using UnityEngine;

public enum PieceColor { Light, Dark }

public class Piece : MonoBehaviour
{
    public PieceColor color;
    public int x; // Spalte auf dem Brett (0-7)
    public int y; // Reihe auf dem Brett (0-7)
    public bool isKing = false;

    // Referenz zum Kronen-Sprite, um es ein- und auszublenden
    [SerializeField] private GameObject kingVisual;

    public void Setup(PieceColor pieceColor, int startX, int startY)
    {
        color = pieceColor;
        x = startX;
        y = startY;
        UpdateKingVisual();
    }

    public void MoveTo(int newX, int newY)
    {
        x = newX;
        y = newY;
    }

    public void PromoteToKing()
    {
        isKing = true;
        Debug.Log($"BEFÖRDERUNG: Stein bei ({x}, {y}) wurde zur Dame!");
        UpdateKingVisual();
    }

    private void UpdateKingVisual()
    {
        if (kingVisual != null)
        {
            kingVisual.SetActive(isKing);
        }
    }
}
