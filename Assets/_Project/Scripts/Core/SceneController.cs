using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    private string _mainSceneName;
    private readonly List<string> _completedMemePointIds = new List<string>();
    private Vector3 _respawnPosition;
    private string _previousSceneName;
    
    private void Awake()
    {
        // Wenn bereits eine Instanz existiert und es nicht diese hier ist,
        // zerstöre dieses Duplikat und verlasse die Methode sofort.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Dies ist die erste Instanz, mache sie zum Singleton.
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Stelle sicher, dass dieses Objekt auch ein EventSystem hat.
        if (GetComponent<EventSystem>() == null)
        {
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void EnterMinigame(string triggerId, Vector3 triggerPosition, string sceneName)
    {
        if (!_completedMemePointIds.Contains(triggerId))
        {
            _completedMemePointIds.Add(triggerId);
        }
        
        _respawnPosition = triggerPosition;
        _mainSceneName = SceneManager.GetActiveScene().name;
        LoadScene(sceneName);
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
        // Prüfen, ob wir zur Hauptszene zurückkehren.
        if (scene.name == _mainSceneName) {
            
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

            // Finde alle MemePoints in der Szene.
            MemePoint[] memePoints = FindObjectsByType<MemePoint>(FindObjectsSortMode.None);
            foreach (MemePoint point in memePoints)
            {
                // Wenn die ID des Punktes in unserer Liste der abgeschlossenen Punkte ist, zerstöre ihn.
                if (_completedMemePointIds.Contains(point.memePointId))
                {
                    Destroy(point.gameObject);
                }
            }
            
            // Setze nur die Respawn-Position zurück. Die Liste der IDs bleibt erhalten.
            _respawnPosition = Vector3.zero;
        }
    }
}
