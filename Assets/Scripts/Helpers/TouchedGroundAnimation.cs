using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchedGroundAnimation : MonoBehaviour
{
    PlayerAnimations playerAnimations;
    PlayerController playerController;

    void  Awake()
    {
        playerAnimations = FindObjectOfType<PlayerAnimations>();
        playerController = FindObjectOfType<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.GetComponent<BoxCollider2D>().IsTouchingLayers(LayerMask.GetMask("Jumpable")) && playerController.moveInput.x == 0)
        {
            playerAnimations.StartAnimation("Idle", true, 1f);
        }
    }

}
