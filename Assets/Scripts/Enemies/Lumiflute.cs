using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lumiflute : EnemyBase
{
    [SerializeField] float moveSpeed = 5f;

    void FixedUpdate()
    {
        Move();
    }

    void Move() {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }
}
