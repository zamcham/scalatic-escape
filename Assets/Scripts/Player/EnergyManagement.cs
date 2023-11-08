using UnityEngine;

public class EnergyManagement : MonoBehaviour
{
    public float currentEnergy { get; private set; }

    public float maxEnergy;
        
    [SerializeField] float startingEnergy;

    [Header("Size Shifting")]
    [Tooltip("In percent.")] public float pixieEnergyThreshold;
    [Tooltip("In percent.")] public float titanEnergyThreshold; // Need to seperate these two, otherwise the header will be applied to both at the same time

    [Tooltip("In percent.")] public float nomadEnergyCost, pixieEnergyCost, titanEnergyCost;

    [Header("UI")]
    [SerializeField] BarUI energyBar;

    void Start()
    {
        currentEnergy = startingEnergy;
        energyBar.SetValue(currentEnergy);
    }

    public void AddEnergy(float energy)
    {
        currentEnergy += energy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        energyBar.SetValue(currentEnergy);
    }

    public void AddEnergyPercent(float percent)
    {
        float energy = maxEnergy * (percent / 100f);

        currentEnergy += energy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        energyBar.SetValue(currentEnergy);
    }
}