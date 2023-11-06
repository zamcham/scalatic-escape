using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float jumpForce = 10f;
    Rigidbody2D rb;
    bool isOnGround = true;
    LayerMask groundLayer = 1 << 8;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
