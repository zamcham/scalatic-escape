using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    // Note: This is a base class for all enemies
    // Box Collider 2D is used for the enemy's body and area that hurts the player
    // Capsule Collider 2D is used for the enemy's head and area that kills the enemy
    
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collision.otherCollider (enemy collider) is not null
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
