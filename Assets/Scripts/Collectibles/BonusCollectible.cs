using UnityEngine;

public class BonusCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] AudioClip collectSound;

    public void OnCollect()
    {
        GameManager.Instance.levelManager.AddBonus();

        // Might add an animation later on
        // In that case, we will disable this script instead of destroying the object

        AudioSource.PlayClipAtPoint(collectSound, transform.position);

        Destroy(gameObject);
    }
}