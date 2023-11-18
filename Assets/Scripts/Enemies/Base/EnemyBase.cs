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
        if (collision.transform.root.CompareTag("Player"))
        {
            // Check if collision.otherCollider (enemy collider) is not null
            if (collision.otherCollider != null)
            {
                // Get the type of the collider
                Type colliderType = collision.otherCollider.GetType();

                // Check the type and perform actions accordingly
                if (colliderType == typeof(BoxCollider2D))
                {
                    Debug.Log($"Enemy {gameObject.name} collided with the player and hurt it");
                    HurtPlayer(collision.gameObject.GetComponent<PlayerController>());
                }
                else if (colliderType == typeof(CapsuleCollider2D))
                {
                    // Handle collision with CapsuleCollider2D
                    Debug.Log($"Enemy {gameObject.name} collided with the player and died");
                    Die();
                }
                else
                {
                    // Handle other collider types if needed
                    Debug.Log($"Collided with a collider of type {colliderType}");
                }
            }
        }  
    }

    protected virtual void Die()
    {
        // Common logic for enemy death
        Destroy(gameObject);
    }

    protected virtual void HurtPlayer(PlayerController player)
    {
        player.Hurt();
    }
}