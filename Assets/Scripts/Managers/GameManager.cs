using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Paused = 0, Running = 1 }
public enum PauseState { PauseMenu = 0, MainMenu = 1, GameOverMenu = 2 }

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isPaused { get { return gameState == GameState.Paused; } }

    // States
    public GameState gameState { get; private set; }
    public PauseState pauseState { get; private set; }

    float timescaleAtPause = 0.0001f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        Debug.Log(gameState);
        Debug.Log(pauseState);
    }

    private void Update()
    {
        }
    }

    void Pause(PauseState pauseState)
    {
        gameState = GameState.Paused;
        this.pauseState = pauseState;

        Time.timeScale = timescaleAtPause;
    }

    void Resume()
    {
        gameState = GameState.Running;

        Time.timeScale = 1f;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Pause(PauseState.MainMenu);
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Pause(PauseState.MainMenu);
    }
}