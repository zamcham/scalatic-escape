using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Count References")]
    public int currentSceneIndex;
    int sceneCount;

    void Awake()
    {
        //Get the current scene index and the total number of scenes
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        sceneCount = SceneManager.sceneCountInBuildSettings;

    }

    private void Update()
    {

    }

    public void LoadLevelMap()
    {
        SceneManager.LoadScene("Levels Map");
    }
}