using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected BoxCollider2D hurtPlayerCollider;
    protected CapsuleCollider2D killEnemyCollider;

    protected virtual void Start()
    {
        // Find and assign colliders during initialization
        FindColliders();
    }

        protected virtual void FindColliders()
    {
        hurtPlayerCollider = transform.Find("hurtPlayerCollider").GetComponent<Collider2D>();
        killEnemyCollider = transform.Find("killEnemyCollider").GetComponent<Collider2D>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check which collider was hit
            if (collision.collider == hurtCollider)
            {
                // Handle player touching the sides of the enemy
                PlayerHurt();
            }
            else if (collision.collider == deathCollider)
            {
                // Handle player jumping on the top of the enemy
                Die();
            }
        }
    }

    protected virtual void Die()
    {
        // Common logic for enemy death
        Destroy(gameObject);
    }

    protected virtual void PlayerHurt()
    {
        // Common logic for when the player touches the sides of the enemy
        // This might include reducing player health or triggering a hurt animation
        Debug.Log("Player hurt");
    }

}
