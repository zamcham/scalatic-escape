using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] BarUI staminaBar;

    // Update is called once per frame
    void Update()
    {
        staminaBar.SetValue(GameSession.Instance.stamina);
    }
}