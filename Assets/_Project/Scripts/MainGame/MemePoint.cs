using UnityEngine;

public class MemePoint : MonoBehaviour
{    
    [Tooltip("Eine eindeutige ID fuer diesen Punkt, z.B. 'MemePoint1'")]
    public string memePointId;

    [Tooltip("Der Name der Minispiel-Szene, die geladen werden soll.")]
    public string minigameSceneName;

    [Tooltip("Das Meme, das durch diesen Punkt freigeschaltet wird.")]
    public MemeData memeToUnlock;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Pruefen, ob das kollidierende Objekt der Spieler ist.
        if (other.CompareTag("Player"))
        {
            // Starte das Minispiel ueber den SceneController und uebergib dieses MemePoint-Objekt.
            SceneController.instance.EnterMinigame(this, transform.position);
        }
    }
}
