using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    
    private GameObject _minigameTriggerObject;
    private string _mainSceneName;
    
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

    public void EnterMinigame(GameObject triggerObject)
    {
        _minigameTriggerObject = triggerObject;
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
        if (scene.name == _mainSceneName && _minigameTriggerObject != null) {
            Destroy(_minigameTriggerObject);
            _minigameTriggerObject = null;
        }
    }
}
