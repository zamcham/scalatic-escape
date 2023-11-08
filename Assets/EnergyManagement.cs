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

        energyBar.SetValue(currentEnergy);
    }
}