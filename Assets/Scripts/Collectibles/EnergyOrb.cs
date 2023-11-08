using UnityEngine;

public class EnergyOrb : MonoBehaviour, ICollectible
{
    [SerializeField] float energyGain;

    public void OnCollect(GameObject collector)
    {
        EnergyManagement energyManager = collector.GetComponent<EnergyManagement>();

        if (energyManager == null) { Debug.Log("The collector does not have an EnergyManagement component!"); return; }

        energyManager.AddEnergy(energyGain);

        // Might add an animation later on
        // In that case, we will disable this script instead of destroying the object
        Destroy(gameObject);
    }
}