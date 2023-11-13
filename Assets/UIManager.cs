using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] RawImage fadeOverlay;
    [SerializeField] Transform levelsMapUI, inGameUI, timerPopup;
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

    public void ShowTimerPopup()
    {
        timerPopup.gameObject.SetActive(true);
    }

    public void HideTimerPopup()
    {
        timerPopup.gameObject.SetActive(false);
    }

    public IEnumerator SceneFadeIn(float duration)
    {
        float lerp = 0f;

        Color initialColor = fadeOverlay.color;

        Color targetColor = initialColor;
        targetColor.a = 1f;

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / duration;
            fadeOverlay.color = Color.Lerp(initialColor, targetColor, lerp);

            yield return null;
        }
    }

    public IEnumerator SceneFadeOut(float duration)
    {
        float lerp = 0f;

        Color initialColor = fadeOverlay.color;

        Color targetColor = initialColor;
        targetColor.a = 0f;

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / duration;
            fadeOverlay.color = Color.Lerp(initialColor, targetColor, lerp);

            yield return null;
        }
    }
}