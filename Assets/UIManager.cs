using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Transform levelsMapUI, inGameUI;
    [SerializeField] BarUI staminaBar;
    [SerializeField] TMP_Text timerText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        switch (GameManager.Instance.gameStatus)
        {
            case GameStatus.OnStartScreen:
                inGameUI.gameObject.SetActive(false);
                break;

            case GameStatus.OnLevelsMap:
                inGameUI.gameObject.SetActive(false);
                levelsMapUI.gameObject.SetActive(true);

                break;

            case GameStatus.InGame:
                inGameUI.gameObject.SetActive(true);
                levelsMapUI.gameObject.SetActive(false);

                if (GameManager.Instance.levelManager)
                {
                    staminaBar.SetValue(GameManager.Instance.levelManager.currentEnergy);
                    timerText.text = GameManager.Instance.levelManager.levelTimer.ToString("0");
                }
                break;
        }
    }
}