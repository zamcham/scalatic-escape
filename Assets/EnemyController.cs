using UnityEngine;

public class EnemyController : MonoBehaviour, IEntity
{
    IEntity entity;

    public float health { get; set; } = 1f;
    public bool hasDied { get; set; } = false;

    public void OnDeath()
    {
        Destroy(gameObject);
    }

    void Awake()
    {
        entity = GetComponent<IEntity>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.root.CompareTag("Player"))
        {
            entity.Kill();
        }
    }
}
