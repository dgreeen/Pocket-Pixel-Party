using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    
    private string _mainSceneName;
    private string _triggerIdToDestroy;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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

    public void EnterMinigame(string triggerId)
    {
        _triggerIdToDestroy = triggerId;
        _mainSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void ReturnToMainGame()
    {
        if (!string.IsNullOrEmpty(_mainSceneName))
        {
            LoadScene(_mainSceneName);
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Prüfen, ob wir zur Hauptszene zurückkehren und ob eine ID zum Zerstören gespeichert ist.
        if (scene.name == _mainSceneName && !string.IsNullOrEmpty(_triggerIdToDestroy)) {
            MemePoint[] memePoints = FindObjectsOfType<MemePoint>();
            foreach (MemePoint point in memePoints)
            {
                if (point.memePointId == _triggerIdToDestroy)
                {
                    Destroy(point.gameObject);
                    break; // Objekt gefunden und zerstört, Schleife beenden.
                }
            }
            _triggerIdToDestroy = null; // ID zurücksetzen, damit es nicht nochmal passiert.
        }
    }
}
