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
        // Singleton-Pattern angepasst:
        // Wenn bereits eine Instanz existiert, zerstören wir die ALTE und behalten die NEUE.
        // Das ist notwendig, weil die UI-Buttons in der Szene mit der NEUEN Instanz verknüpft sind.
        if (Instance != null && Instance != this)
        {
            // Übernehme die Auswahl der alten Instanz, falls in der neuen noch nichts gesetzt ist.
            if (selectedCharacterPrefab == null)
            {
                selectedCharacterPrefab = Instance.selectedCharacterPrefab;
            }
            Destroy(Instance.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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
                SceneManager.LoadScene("SampleScene"); 
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
            SceneManager.LoadScene("MainMenu"); 
        }
    }
}
