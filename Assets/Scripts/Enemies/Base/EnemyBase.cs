using System;
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
        Debug.Log("EnemyBase Start");
    }

    protected virtual void FindColliders()
    {
        hurtPlayerCollider = GetComponent<BoxCollider2D>();
        killEnemyCollider = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collision.otherCollider is not null
        if (collision.otherCollider != null)
        {
            // Get the type of the collider
            Type colliderType = collision.otherCollider.GetType();

            // Check the type and perform actions accordingly
            if (colliderType == typeof(BoxCollider2D))
            {
                // Handle collision with BoxCollider2D
                Debug.Log("Collided with BoxCollider2D");
                PlayerHurt();
            }
            else if (colliderType == typeof(CapsuleCollider2D))
            {
                // Handle collision with CapsuleCollider2D
                Debug.Log("Collided with CapsuleCollider2D");
                Die();
            }
            else
            {
                // Handle other collider types if needed
                Debug.Log($"Collided with a collider of type {colliderType}");
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
