using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] Transform levelsMapUI, inGameUI;
    [SerializeField] BarUI staminaBar;

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
        switch (GameManager.Instance.gameStatus)
        {
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
                }
                break;
        } 
    }
}