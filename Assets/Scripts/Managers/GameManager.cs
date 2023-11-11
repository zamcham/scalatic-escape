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
    
    //Dictionary of levels and their completion status
    public Dictionary<int, bool> levelStatus;

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
    }
    void Start()
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
        SceneManager.LoadScene("Levels Map");
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
                //TODO: Add level locked animation
                Debug.Log($"Level {levelNumber} is not unlocked yet.");
            }
            else
            {
                //TODO: Handle error
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

    #endregion  

}