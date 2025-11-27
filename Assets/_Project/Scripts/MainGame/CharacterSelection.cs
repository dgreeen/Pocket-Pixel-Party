using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    // Singleton-Instanz, um von überall darauf zugreifen zu können
    public static CharacterSelection Instance;

    // Prefabs für die Charakterauswahl
    public GameObject[] characterPrefabs;
    public GameObject selectedCharacterPrefab;

    private void Awake()
    {
        // Singleton-Pattern: Stellt sicher, dass es nur eine Instanz dieses Objekts gibt
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Dieses Objekt nicht zerstören, wenn eine neue Szene geladen wird
        }
        else
        {
            Destroy(gameObject); // Wenn bereits eine Instanz existiert, diese hier zerstören
        }
    }

    public void SelectCharacter(int characterIndex)
    {
        if (characterIndex >= 0 && characterIndex < characterPrefabs.Length)
        {
            selectedCharacterPrefab = characterPrefabs[characterIndex];
            Debug.Log("Charakter ausgewählt: " + selectedCharacterPrefab.name);

            // Lade die Spiel-Szene über den SceneController
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
