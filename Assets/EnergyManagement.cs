using UnityEngine;

public class EnergyManagement : MonoBehaviour
{
    public float currentEnergy { get; private set; }

    [SerializeField] float maxEnergy, startingEnergy;
    [SerializeField] float pixieEnergyThreshold, titanEnergyThreshold;


    void Start()
    {
        currentEnergy = startingEnergy;
    }

    void Update()
    {
        
    }
}