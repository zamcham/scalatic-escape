using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int currentSceneIndex;
    int totalSceneCount;
    
    //Dictionary of levels and their completion status
    public Dictionary<int, bool> levelStatus;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

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

    public void LoadLevel(int levelNumber)
    {
        if (levelStatus.ContainsKey(levelNumber))
        {
            if (levelStatus[levelNumber])
            {
                SceneManager.LoadScene(levelNumber);
                Debug.Log($"Level {levelNumber} is successfully loaded.");
            }
            else
            {
                Debug.Log($"Level {levelNumber} is not unlocked yet.");
            }
        }
        else
        {
            Debug.Log($"There is no such level numbered {levelNumber}");
        }
    }
}