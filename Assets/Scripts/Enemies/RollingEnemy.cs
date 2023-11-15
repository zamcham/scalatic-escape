using UnityEngine;

public class RollingEnemy : EnemyBase
{
    [SerializeField] Transform sprite;
    float movementDirection, rollSpeed, moveSpeed;

    private void Update()
    {
        movementDirection = 1f;
        rollSpeed = 60f;
        moveSpeed = 2f;
        RollAnimation();
    }

    void RollAnimation()
    {
        sprite.Rotate(Vector3.forward * movementDirection * rollSpeed * Time.deltaTime);
    }
}