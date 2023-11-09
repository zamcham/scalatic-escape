using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Paused = 0, Running = 1 }
public enum PauseState { PauseMenu = 0, MainMenu = 1, GameOverMenu = 2 }

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // States
    public GameState gameState { get; private set; }
    public PauseState pauseState { get; private set; }

    float timescaleAtPause = 0.0001f;
    public bool isPaused { get { return gameState == GameState.Paused; } }

    // Levels
    List<int> completedLevels;
    int currentLevelIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            completedLevels = new List<int>();

            // We will change this to read from JSON data when implementing the save system
            // For now, it will stay at zero
            currentLevelIndex = 0;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
            return;
        }

        /* We made the GameManager the child of the Managers object in the hierarchy for a more organized look
         DontDestroyOnLoad doesn't work with child objects. We only need it to be a child in the editor, so 
         un-parent it here */

        transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        Pause(PauseState.MainMenu);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch (gameState)
            {
                case GameState.Paused:
                    Resume();
                    break;
                    

                case GameState.Running:
                    Pause(PauseState.MainMenu);
                    break;
            }
        }
    }

    public void StartScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);

        Resume();
    }

    public void Pause(PauseState pauseState)
    {
        gameState = GameState.Paused;
        this.pauseState = pauseState;

        Time.timeScale = timescaleAtPause;
    }

    public void Resume()
    {
        gameState = GameState.Running;

        Time.timeScale = 1f;
    }

    // EXTERNAL & PUBLIC
    public void ReloadScene()
    {
        SceneManager.LoadScene(currentLevelIndex);
        Pause(PauseState.MainMenu);
    }

    public void LoadNextScene()
    {
        completedLevels.Add(currentLevelIndex);

        currentLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;

        ReloadScene();
    }
}