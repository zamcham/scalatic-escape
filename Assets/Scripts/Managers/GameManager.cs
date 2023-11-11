using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentSceneIndex;
    int totalSceneCount;
    public LevelManager levelManager;
    
    // Dictionary of levels and their completion status
    public Dictionary<int, bool> levelStatus;

    const string LevelsMapSceneName = "Levels Map";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        totalSceneCount = SceneManager.sceneCountInBuildSettings;

        InitializeLevelStatus();
    }

    void Start()
    {
        
    }

    private void InitializeLevelStatus()
    {
        levelStatus = new Dictionary<int, bool>
        {
            {1, true},
            {2, false},
            {3, false}
        };
    }

    public void LoadLevelsMap()
    {
        SceneManager.LoadScene(LevelsMapSceneName);
    }

    #region Level Handling
    public void LoadLevel(int levelNumber)
    {
        if (LevelIsUnlocked(levelNumber))
        {
            currentSceneIndex = levelNumber;
            SceneManager.LoadScene(levelNumber);
        }
        else
        {
            if (LevelExists(levelNumber))
            {
                // TODO: Add level locked animation
                Debug.Log($"Level {levelNumber} is not unlocked yet.");
            }
            else
            {
                // TODO: Handle error
                Debug.Log($"There is no such level numbered {levelNumber}");
            }
        }
    }

    private bool LevelIsUnlocked(int levelNumber)
    {
        return levelStatus.TryGetValue(levelNumber, out bool isUnlocked) && isUnlocked;
    }

    private bool LevelExists(int levelNumber)
    {
        return levelStatus.ContainsKey(levelNumber);
    }

    //TODO: test this
    public void RestartLevel()
    {
        if (levelManager.checkpointReached)
        {
            Debug.Log("Respawning on checkpoint");
            levelManager.RespawnOnCheckpoint();
        }
        else
        {
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

    #endregion  
}
