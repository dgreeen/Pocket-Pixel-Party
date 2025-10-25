using UnityEngine;

public class MemePoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneController.instance.EnterMinigame();
        }
    }
}
