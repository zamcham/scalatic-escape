using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelButton : MonoBehaviour
{
    TextMeshProUGUI buttonText;

    void Start()
    {
        ChangeButtonNameToLevelNumber();

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
}
