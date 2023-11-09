using UnityEngine;

public class EnergyManagement : MonoBehaviour
{
    public float currentStamina 
    { 
        get 
        {
            return GameSession.Instance.stamina;
        } 

        private set
        {
            GameSession.Instance.stamina = value;
        } 
    }

    public float maxStamina;
        
    [SerializeField] float startingStamina;

    [Header("Size Shifting")]
    [Tooltip("In percent.")] public float pixieEnergyThreshold;
    [Tooltip("In percent.")] public float titanEnergyThreshold; // Need to seperate these two, otherwise the header will be applied to both at the same time

    [Tooltip("In percent.")] public float nomadEnergyCost, pixieEnergyCost, titanEnergyCost;

    void Start()
    {
        currentStamina = startingStamina;
    }

    public void AddStamina(float energy)
    {
        currentStamina += energy;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }

    public void AddStaminaPercent(float percent)
    {
        float stamina = maxStamina * (percent / 100f);

        currentStamina += stamina;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
    }
}