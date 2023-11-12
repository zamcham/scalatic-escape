using UnityEngine;

public class EnergyOrb : MonoBehaviour, ICollectible
{
    [Tooltip("In percent.")][SerializeField] float energyGain;
    [SerializeField] AudioClip collectSound;

    public void OnCollect()
    {
        GameManager.Instance.levelManager.AddEnergyPercent(energyGain);

        // Might add an animation later on
        // In that case, we will disable this script instead of destroying the object

        AudioSource.PlayClipAtPoint(collectSound, transform.position);

        Destroy(gameObject);
    }
}