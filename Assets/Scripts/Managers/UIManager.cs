using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] BarUI staminaBar;
    [SerializeField] DisplayUI scoreDisplay;

    GameSession gameSession;

    private void Awake()
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

        gameSession = GameSession.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        staminaBar.SetValue(gameSession.stamina);
        scoreDisplay.SetDisplay(gameSession.score);
    }
}