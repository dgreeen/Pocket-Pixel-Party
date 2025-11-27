// Dateipfad: Assets/_Project/Scripts/UI/MemeEntryUI.cs
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MemeEntryUI : MonoBehaviour
{
    [SerializeField] private Image memeImage;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = Color.black;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void Setup(MemeData memeData, bool isUnlocked, MemeGalleryUI galleryController)
    {
        if (memeData == null) return;

        // Sprite zuweisen
        memeImage.sprite = memeData.memeSprite;

        // Farbe basierend auf Freischalt-Status anpassen
        memeImage.color = isUnlocked ? unlockedColor : lockedColor;

        // Optional: Seitenverhältnis beibehalten
        memeImage.preserveAspect = true;

        // Button-Logik
        _button.onClick.RemoveAllListeners(); // Alte Listener entfernen
        if (isUnlocked)
        {
            _button.onClick.AddListener(() => galleryController.ShowMemeDetails(memeData));
        }
    }
}
