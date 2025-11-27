using UnityEngine;

public class MemeGalleryUI : MonoBehaviour
{
    [SerializeField] private MemeDatabase memeDatabase;
    [SerializeField] private Transform contentParent; // Das "Content"-Objekt der ScrollView
    [SerializeField] private GameObject memeEntryPrefab; // Unser Prefab von Schritt 2

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

        if (memeDatabase == null || PlayerProfile.instance == null) return;

        // Alle Memes aus der Datenbank durchgehen
        foreach (MemeData meme in memeDatabase.allMemes)
        {
            // Prefab instanziieren
            GameObject entryGO = Instantiate(memeEntryPrefab, contentParent);
            
            // Prüfen, ob das Meme freigeschaltet ist
            bool isUnlocked = PlayerProfile.instance.HasUnlockedMeme(meme.memeId);
            
            // Den Eintrag mit den Daten füllen
            entryGO.GetComponent<MemeEntryUI>().Setup(meme, isUnlocked);
        }
    }
}
