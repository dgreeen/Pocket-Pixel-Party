using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemeUnlockUI : MonoBehaviour
{
    [Header("UI-Elemente")]
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private Image memeImage;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI memeNameText;

    [Header("Einstellungen")]
    [SerializeField] private float displayDuration = 8f;

    private void Awake()
    {
        // Panel zu Beginn ausblenden
        unlockPanel.SetActive(false);

        // Auf das Event lauschen, das bei einer Freischaltung ausgelöst wird. Awake() wird vor Start() aufgerufen.
        if (PlayerProfile.instance != null)
        {
            PlayerProfile.instance.OnMemeDataUnlocked += HandleMemeUnlocked;
        }
        else
        {
            Debug.LogError("MemeUnlockUI konnte PlayerProfile.instance nicht finden, um das Event zu abonnieren!");
        }
    }

    private void OnDestroy()
    {
        // Wichtig: Event-Abonnement beim Zerstören des Objekts wieder aufheben.
        PlayerProfile.instance.OnMemeDataUnlocked -= HandleMemeUnlocked;
    }

    private void HandleMemeUnlocked(MemeData unlockedMeme)
    {
        Debug.Log("MemeUnlockUI hat das Event 'Meme freigeschaltet' empfangen!");
        if (unlockedMeme != null && unlockedMeme.memeSprite != null)
        {
            memeImage.sprite = unlockedMeme.memeSprite;
            memeNameText.text = unlockedMeme.displayName;
            StartCoroutine(ShowUnlockPanel());
        }
    }

    private IEnumerator ShowUnlockPanel() {
        unlockPanel.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        unlockPanel.SetActive(false);
    }
}