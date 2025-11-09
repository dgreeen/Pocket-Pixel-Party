
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MemePoint : MonoBehaviour
{
    [Tooltip("Eine eindeutige ID für diesen MemePoint, z.B. 'MemePoint_Level1_01'")]
    public string memePointId;

    [Tooltip("Der Name der Minispiel-Szene, die geladen werden soll (z.B. 'Pong' oder 'Minesweeper').")]
    public string sceneToLoad;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneController.instance.EnterMinigame(memePointId, transform.position, sceneToLoad);
        }
    }
}
