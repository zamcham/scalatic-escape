using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum GameStatus { OnStartScreen, OnLevelsMap, InGame }

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int currentSceneIndex;
    int totalSceneCount;
    public LevelManager levelManager;

    // Game Status
    
    public GameStatus gameStatus = GameStatus.OnStartScreen;

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

        gameStatus = GameStatus.OnLevelsMap;
    }

    public AsyncOperation LoadLevelsMapAsync()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(LevelsMapSceneName);
        gameStatus = GameStatus.OnLevelsMap;

        return async;
    }

    public AsyncOperation ReloadLevelAsync()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(currentSceneIndex);

        return async;
    }

    #region Level Handling
    public void LoadLevel(int levelNumber)
    {
        if (LevelIsUnlocked(levelNumber))
        {
            currentSceneIndex = levelNumber;
            SceneManager.LoadScene(levelNumber);

            gameStatus = GameStatus.InGame;
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

    public AsyncOperation LoadLevelAsync(int levelNumber)
    {
        if (LevelIsUnlocked(levelNumber))
        {
            gameStatus = GameStatus.InGame;

            currentSceneIndex = levelNumber;
            AsyncOperation async = SceneManager.LoadSceneAsync(levelNumber);

            return async;
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

        return null;
    }

    private bool LevelIsUnlocked(int levelNumber)
    {
        return levelStatus.TryGetValue(levelNumber, out bool isUnlocked) && isUnlocked;
    }

    private bool LevelExists(int levelNumber)
    {
        return levelStatus.ContainsKey(levelNumber);
    }

    public void RestartLevel(UnityEvent customReset = null)
    {
        if (!levelManager.sceneLoading && !levelManager.respawning)
        {
            if (levelManager.checkpointReached)
            {
                levelManager.RespawnOnCheckpoint(customReset);
            }
            else
            {
                levelManager.ReloadLevel();
            }
        }        
    }

    #endregion  
}

public class Level
{
    public int levelNumber;
    public bool levelUnlocked;
    public bool levelCompleted;

    private int _collectedBonuses;
    public int collectedBonuses
    {
        get
        {
            return _collectedBonuses;
        }

        set
        {
            _collectedBonuses = value;

            // All bonuses are collected
            if (_collectedBonuses >= 3)
            {
                bonusesCompleted = true;
            }

            // Not collected
            else
            {
                bonusesCompleted = false;
            }
        }
    }
    public bool bonusesCompleted;
}