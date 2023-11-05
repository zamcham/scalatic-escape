using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float jumpForce = 10f;
    Rigifbody playerRb;
    bool isOnGround = true;
    LayerMask groundLayer = 1 << 8;

    void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
    }
}
