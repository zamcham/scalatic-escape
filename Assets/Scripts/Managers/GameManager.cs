using UnityEngine;

public enum GameState { Paused = 0, Running = 1 }
public enum PauseState { PauseMenu = 0, MainMenu = 1, GameOverMenu = 2 }

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isPaused { get { return gameState == GameState.Paused; } }

    // States
    public GameState gameState { get; private set; } = GameState.Paused;
    public PauseState pauseState { get; private set; } = PauseState.MainMenu;

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
        }

        /* We made the GameManager the child of the Managers object in the hierarchy for a more organized look
         DontDestroyOnLoad doesn't work with child objects. We only need it to be a child in the editor, so 
         un-parent it on the awake */

        transform.SetParent(null);

        DontDestroyOnLoad(gameObject);
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
}