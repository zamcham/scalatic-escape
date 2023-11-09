using System.Collections;
using System.Collections.Generic;
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

    void Update()
    {
        
    }

    void ChangeButtonNameToLevelNumber()
    {
        buttonText = transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();

        if (buttonText.text.StartsWith("Level "))
        {
            string number = buttonText.text.Substring(6);
            gameObject.name = number;
        }
    }

    void AddListenerToButton()
    {
        Debug.Log("AddListenerToButton");
        if (buttonComponent != null && gameManager != null)
        {
            string buttonName = gameObject.name;
            int levelNumber = int.Parse(buttonName);
            buttonComponent.onClick.AddListener(() => gameManager.LoadLevel(levelNumber));
            Debug.Log("success");
        }
    }
}
