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
    private string _currentMemePointId; // Merkt sich die ID des aktuellen Minispiels
    private MemeData _currentMemeToUnlock; // Merkt sich das Meme, das freigeschaltet werden kann
    private string _previousSceneName;
    private bool _minigameWasWon = false; // NEU: Merker für den Sieg
    
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

    public void EnterMinigame(MemePoint memePoint, Vector3 triggerPosition)
    {
        _currentMemePointId = memePoint.memePointId;
        _currentMemeToUnlock = memePoint.memeToUnlock;
        _respawnPosition = triggerPosition;
        _mainSceneName = SceneManager.GetActiveScene().name;
        LoadScene(memePoint.minigameSceneName);
    }

    public void UncompleteCurrentMinigame()
    {
        if (!string.IsNullOrEmpty(_currentMemePointId))
        {
            _completedMemePointIds.Remove(_currentMemePointId);
        }
    }

    // Wird von Minispielen aufgerufen, die eine explizite Siegbedingung haben.
    public void CompleteCurrentMinigame()
    {
        // Merke dir nur, dass das Spiel gewonnen wurde. Die Belohnung kommt später.
        _minigameWasWon = true;
    }

    // Wird von Minispielen aufgerufen, wenn sie beendet sind (Sieg oder Niederlage).
    // Markiert den MemePoint, damit er zerstört wird.
    public void FinishCurrentMinigameAttempt()
    {
        if (!string.IsNullOrEmpty(_currentMemePointId) && !_completedMemePointIds.Contains(_currentMemePointId))
        {
            _completedMemePointIds.Add(_currentMemePointId);
        }
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
                // Setze den Spieler auf die exakte Respawn-Position zurück.
                player.transform.position = _respawnPosition;
            }

            // NEU: Prüfen, ob eine Belohnung aussteht.
            if (_minigameWasWon && PlayerProfile.instance != null)
            {
                // Jetzt, wo wir in der Hauptszene sind und die UI existiert,
                // schalten wir das Meme frei. Das löst das UI-Event aus.
                PlayerProfile.instance.UnlockMeme(_currentMemeToUnlock);
            }

            // Setze den Gewinn-Status für das nächste Minigame zurück.
            _minigameWasWon = false;

            // Finde alle MemePoints in der Szene.
            MemePoint[] memePoints = FindObjectsByType<MemePoint>(FindObjectsSortMode.None);
            foreach (MemePoint point in memePoints)
            {
                // Wenn die ID des Punktes in unserer Liste der abgeschlossenen Punkte ist, zerstöre ihn.
                // Stelle sicher, dass die ID nicht leer ist, bevor du prüfst.
                if (!string.IsNullOrEmpty(point.memePointId) && _completedMemePointIds.Contains(point.memePointId))
                {                    
                    Destroy(point.gameObject);
                }
            }
            // Setze nur die Respawn-Position zurück. Die Liste der IDs bleibt erhalten.
            _respawnPosition = Vector3.zero;
            _currentMemePointId = null;
            _currentMemeToUnlock = null;
        }
    }

}