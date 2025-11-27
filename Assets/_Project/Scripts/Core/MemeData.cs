using UnityEngine;

[CreateAssetMenu(fileName = "New Meme", menuName = "Meme Achievement/Meme Data")]
public class MemeData : ScriptableObject
{
    [Tooltip("Eine eindeutige ID für dieses Meme, z.B. 'surprised_pikachu'")]
    public string memeId;

    [Tooltip("Der Name, der im Spiel angezeigt wird, z.B. 'Surprised Pikachu'")]
    public string displayName;

    [Tooltip("Eine kurze Beschreibung oder ein Zitat, das zum Meme passt.")]
    [TextArea(3, 5)]
    public string description;

    [Tooltip("Das Bild/Sprite für dieses Meme")]
    public Sprite memeSprite;

    // Hier könntest du später weitere Infos hinzufügen,
    // z.B. eine Beschreibung, Seltenheit, etc.
}
