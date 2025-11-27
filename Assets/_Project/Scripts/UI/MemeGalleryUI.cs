using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class MemeGalleryUI : MonoBehaviour
{
    [SerializeField] private MemeDatabase memeDatabase;
    [SerializeField] private Transform contentParent; // Das "Content"-Objekt der ScrollView
    [SerializeField] private GameObject memeEntryPrefab; // Unser Prefab von Schritt 2

    [Header("Detailansicht")]
    [Tooltip("Das Bild, das das ausgewählte Meme groß anzeigt.")]
    [SerializeField] private Image detailImage;
    [Tooltip("Der Text für den Namen des ausgewählten Memes.")]
    [SerializeField] private TextMeshProUGUI detailNameText;
    [Tooltip("Der Text für die Beschreibung des ausgewählten Memes.")]
    [SerializeField] private TextMeshProUGUI detailDescriptionText;

    // Diese Methode wird aufgerufen, wenn das Panel aktiviert wird.
    void OnEnable()
    {
        PopulateGallery();
    }

    public void PopulateGallery()
    {
        // Zuerst alle alten Einträge löschen, falls die Galerie neu aufgebaut wird
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        if (memeDatabase == null || PlayerProfile.instance == null)
        {
            Debug.LogError("MemeDatabase oder PlayerProfile nicht gefunden!");
            return;
        }

        // Finde alle freigeschalteten Memes, um sie später zu verwenden.
        List<MemeData> unlockedMemes = memeDatabase.allMemes
            .Where(meme => PlayerProfile.instance.HasUnlockedMeme(meme.memeId))
            .ToList();

        // Alle Memes aus der Datenbank durchgehen
        foreach (MemeData meme in memeDatabase.allMemes)
        {
            // Prefab instanziieren
            GameObject entryGO = Instantiate(memeEntryPrefab, contentParent);

            // Prüfen, ob das Meme freigeschaltet ist
            bool isUnlocked = PlayerProfile.instance.HasUnlockedMeme(meme.memeId);

            // Den Eintrag mit den Daten füllen
            entryGO.GetComponent<MemeEntryUI>().Setup(meme, isUnlocked, this);
        }

        // Wenn es freigeschaltete Memes gibt, zeige das erste in der Detailansicht.
        // Ansonsten leere die Ansicht.
        if (unlockedMemes.Count > 0)
        {
            ShowMemeDetails(unlockedMemes[0]);
        }
        else
        {
            ClearDetailView();
        }
    }

    /// <summary>
    /// Aktualisiert die Detailansicht mit den Informationen des übergebenen Memes.
    /// </summary>
    public void ShowMemeDetails(MemeData meme)
    {
        if (meme == null) return;

        detailImage.sprite = meme.memeSprite;
        detailNameText.text = meme.displayName;
        detailDescriptionText.text = meme.description;
        detailImage.gameObject.SetActive(true);
    }

    private void ClearDetailView()
    {
        detailImage.gameObject.SetActive(false);
        detailNameText.text = "Meme-Galerie";
        detailDescriptionText.text = "Spiele Minispiele, um Memes freizuschalten!";
    }
}
