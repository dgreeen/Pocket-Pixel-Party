using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    // Singleton-Instanz, um von ueberall darauf zugreifen zu koennen
    public static CharacterSelection Instance;

    // Prefabs fuer die Charakterauswahl
    public GameObject[] characterPrefabs;
    public GameObject selectedCharacterPrefab;

    private void Awake()
    {
        // Singleton-Pattern: Stellt sicher, dass es nur eine Instanz dieses Objekts gibt
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Dieses Objekt nicht zerstoeren, wenn eine neue Szene geladen wird
        }
        else
        {
            Destroy(gameObject); // Wenn bereits eine Instanz existiert, diese hier zerstoeren
        }
    }

    public void SelectCharacter(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterPrefabs.Length)
        {
            selectedCharacterPrefab = characterPrefabs[characterIndex];
            Debug.Log("Charakter ausgewaehlt: " + selectedCharacterPrefab.name);

            // Lade die Spiel-Szene ueber den SceneController
            if (SceneController.instance != null)
            {
                // "SampleScene" ist der Name deiner Hauptspiel-Szene
                SceneController.instance.LoadScene("SampleScene");
            }
            else
            {
                Debug.LogError("SceneController nicht gefunden!");
                SceneManager.LoadScene("SampleScene"); // Fallback
            }
        }
    }

    public void GoToMainMenu()
    {
        if (SceneController.instance != null)
        {
            SceneController.instance.LoadScene("MainMenu");
        }
        else
        {
            Debug.LogError("SceneController nicht gefunden!");
            SceneManager.LoadScene("MainMenu"); // Fallback
        }
    }
}
