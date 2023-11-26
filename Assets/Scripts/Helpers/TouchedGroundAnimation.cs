using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchedGroundAnimation : MonoBehaviour
{
    PlayerAnimations playerAnimations;

    void  Awake()
    {
        playerAnimations = FindObjectOfType<PlayerAnimations>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Jumpable")))
        {
            playerAnimations.StartAnimation("Idle", true, 1f);
        }
    }

}
