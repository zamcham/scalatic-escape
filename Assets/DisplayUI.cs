using UnityEngine;
using TMPro;

public class DisplayUI : MonoBehaviour
{
    [SerializeField] TMP_Text displayText;

    public void SetDisplay(string display)
    {
        if (displayText.text == display) { return; }

        displayText.text = display;
    }

    public void SetDisplay(int intDisplay)
    {
        string display = intDisplay.ToString();

        if (displayText.text == display) { return; }

        displayText.text = display;
    }
}