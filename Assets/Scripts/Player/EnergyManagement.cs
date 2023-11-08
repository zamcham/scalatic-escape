using UnityEngine;

public class EnergyManagement : MonoBehaviour
{
    public float currentEnergy { get; private set; }

    [SerializeField] float maxEnergy, startingEnergy;

    public float pixieEnergyCost, titanEnergyCost;

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