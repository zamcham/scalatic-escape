using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Menus")]
    [SerializeField] GameObject hud;
    [SerializeField] GameObject pauseMenu, mainMenu, gameOverMenu;

    [Header("Value Displays")]
    [SerializeField] BarUI staminaBar;
    [SerializeField] DisplayUI scoreDisplay;

    GameManager gameManager;
    GameSession gameSession;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameManager = GameManager.Instance;
        gameSession = GameSession.Instance;
    }

    void Update()
    {
        switch (gameManager.gameState)
        {
            case GameState.Paused:

                switch (gameManager.pauseState)
                {
                    case PauseState.PauseMenu:
                        ActivatePauseMenu();
                        break;

                    case PauseState.MainMenu:
                        ActivateMainMenu();
                        break;

                    case PauseState.GameOverMenu:
                        ActivateGameOverMenu();
                        break;
                }

                break;

            case GameState.Running:
                DisableMenus();
                UpdateValues();
                break;
        }        
    }

    #region Menus
    void ActivatePauseMenu()
    {
        pauseMenu.SetActive(true);
    }

    void ActivateMainMenu()
    {
        mainMenu.SetActive(true);

        hud.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }

    void ActivateGameOverMenu()
    {
        gameOverMenu.SetActive(true);

        hud.SetActive(false);
    }

    void DisableMenus()
    {
        hud.SetActive(true);

        mainMenu.SetActive(false);
        pauseMenu.SetActive(false);
        gameOverMenu.SetActive(false);
    }
    #endregion

    void UpdateValues()
    {
        staminaBar.SetValue(gameSession.stamina);
        scoreDisplay.SetDisplay(gameSession.score);
    }

    // Public wrapper methods to call from the UI button events
    public void StartLevel()
    {
        // Implement this fully
        //gameManager.StartScene();
        ResumeLevel();
    }

    public void RestartLevel()
    {
        gameManager.ReloadScene();
    }

    public void ResumeLevel()
    {
        gameManager.Resume();
    }
}