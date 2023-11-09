using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] BarUI staminaBar;
    [SerializeField] DisplayUI scoreDisplay;

    GameSession gameSession;

    private void Awake()
    {
        gameSession = GameSession.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        staminaBar.SetValue(gameSession.stamina);
        scoreDisplay.SetDisplay(gameSession.score);
    }
}