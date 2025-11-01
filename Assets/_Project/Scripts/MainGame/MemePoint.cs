
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MemePoint : MonoBehaviour
{
    [Tooltip("Eine eindeutige ID für diesen MemePoint, z.B. 'MemePoint_Level1_01'")]
    public string memePointId;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneController.instance.EnterMinigame(memePointId);
        }
    }
}
