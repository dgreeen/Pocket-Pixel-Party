using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    
    private string _mainSceneName;
    private string _triggerIdToDestroy;
    private Vector3 _respawnPosition;
    private string _previousSceneName;
    
    private void Awake()
    {
        // Klassisches Singleton-Pattern: Stellt sicher, dass es nur einen SceneController gibt.
        if (instance == null)
        {
            instance = this;
            // Stelle sicher, dass dieses Objekt auch ein EventSystem hat.
            if (GetComponent<EventSystem>() == null)
            {
                gameObject.AddComponent<EventSystem>();
                gameObject.AddComponent<StandaloneInputModule>();
            }
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EnterMinigame(string triggerId, Vector3 triggerPosition)
    {
        _triggerIdToDestroy = triggerId;
        _respawnPosition = triggerPosition;
        _mainSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(string sceneName)
    {
        _previousSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void ReturnToMainGame()
    {
        // Diese Methode ist spezifisch für die Rückkehr aus Minispielen
        if (!string.IsNullOrEmpty(_mainSceneName)) 
        {
            LoadScene(_mainSceneName);
        }
        else
        {
            Debug.LogWarning("Keine Hauptspiel-Szene zum Zurückkehren definiert. Verwende ReturnToPreviousScene().");
            ReturnToPreviousScene();
        }
    }

    public void ReturnToPreviousScene()
    {
        if (!string.IsNullOrEmpty(_previousSceneName))
        {
            LoadScene(_previousSceneName);
        }
        else
        {
            Debug.LogError("Keine vorherige Szene zum Zurückkehren gefunden!");
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Prüfen, ob wir zur Hauptszene zurückkehren und ob eine ID zum Zerstören gespeichert ist.
        if (scene.name == _mainSceneName && !string.IsNullOrEmpty(_triggerIdToDestroy)) {
            
            // Finde den Spieler und setze seine Position
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = _respawnPosition;

                // Aktualisiere den Respawn-Punkt im PlayerRespawn-Skript
                PlayerRespawn playerRespawn = player.GetComponent<PlayerRespawn>();
                if (playerRespawn != null) {
                    playerRespawn.SetRespawnPoint(_respawnPosition);
                }
            }

            MemePoint[] memePoints = FindObjectsByType<MemePoint>(FindObjectsSortMode.None);
            foreach (MemePoint point in memePoints)
            {
                if (point.memePointId == _triggerIdToDestroy)
                {
                    Destroy(point.gameObject);
                    break; // Objekt gefunden und zerstört, Schleife beenden.
                }
            }
            _triggerIdToDestroy = null; // ID und Position zurücksetzen, damit es nicht nochmal passiert.
        }
    }
}
