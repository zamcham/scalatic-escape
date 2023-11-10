using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    TextMeshProUGUI buttonText;
    Button buttonComponent;
    GameManager gameManager;

    void Awake()
    {
        ChangeButtonNameToLevelNumber();
        gameManager = FindObjectOfType<GameManager>();
        buttonComponent = GetComponent<Button>();
    }

    void Start()
    {
        AddListenerToButton();
    }

    void ChangeButtonNameToLevelNumber()
    {
        buttonText = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        // Using a regex to get only the numbers
        string number = Regex.Replace(buttonText.text, "[^0-9]", "");
        gameObject.name = number;
    }

    void AddListenerToButton()
    {
        if (buttonComponent != null && gameManager != null)
        {
            string buttonName = gameObject.name;
            int levelNumber = int.Parse(buttonName);
            buttonComponent.onClick.AddListener(() => gameManager.LoadLevel(levelNumber));
        }
    }
}