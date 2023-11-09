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
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        totalSceneCount = SceneManager.sceneCountInBuildSettings;
        DontDestroyOnLoad(this.gameObject);
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevelsMap()
    {
        SceneManager.LoadScene("Levels Map");
    }

    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene(levelNumber);
    }
}
