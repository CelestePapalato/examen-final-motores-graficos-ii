using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField]
    LevelCollection levelCollection;

    string current_level_tag;

    public UnityEvent OnSceneLoad;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void ChangeLevelTag(string level_tag)
    {
        current_level_tag = level_tag;
    }

    public void LoadScene()
    {
        if (current_level_tag == null) { return; }
        LevelData level = levelCollection.Levels.First(x => x.level_tag == current_level_tag);
        if(level == null) { return; }
        OnSceneLoad?.Invoke();
        SceneManager.LoadScene(level.scene.name);
    }
    public void ReloadLevel()
    {
        OnSceneLoad?.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
