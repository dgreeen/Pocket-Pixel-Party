using UnityEngine;

public class MemePoint : MonoBehaviour
{    
    [Tooltip("Eine eindeutige ID für diesen Punkt, z.B. 'MemePoint1'")]
    public string memePointId;

    [Tooltip("Der Name der Minispiel-Szene, die geladen werden soll.")]
    public string minigameSceneName;

    [Tooltip("Das Meme, das durch diesen Punkt freigeschaltet wird.")]
    public MemeData memeToUnlock;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Prüfen, ob das kollidierende Objekt der Spieler ist.
        if (other.CompareTag("Player"))
        {
            // Starte das Minispiel über den SceneController und übergib dieses MemePoint-Objekt.
            SceneController.instance.EnterMinigame(this, transform.position);
        }
    }
}
