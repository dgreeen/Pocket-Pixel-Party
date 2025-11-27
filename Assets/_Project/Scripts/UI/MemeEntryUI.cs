// Dateipfad: Assets/_Project/Scripts/UI/MemeEntryUI.cs
using UnityEngine;
using UnityEngine.UI;

public class MemeEntryUI : MonoBehaviour
{
    [SerializeField] private Image memeImage;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.black;

    public void Setup(MemeData memeData, bool isUnlocked)
    {
        // Sprite zuweisen
        memeImage.sprite = memeData.memeSprite;
        
        // Farbe basierend auf Freischalt-Status anpassen
        memeImage.color = isUnlocked ? unlockedColor : lockedColor;
        
        // Optional: Seitenverhältnis beibehalten
        memeImage.preserveAspect = true;
    }
}
