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
    public static UnityAction OnSceneLoading;

    private void Awake()
    {
        if (Instance != this && Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
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
        InventorySystem.Inventory.Clear();
        Time.timeScale = 1f;
        SceneManager.LoadScene(level.scene);
    }
    public void ReloadLevel()
    {
        OnSceneLoad?.Invoke();
        InventorySystem.Inventory.Clear();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CloseGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
}
