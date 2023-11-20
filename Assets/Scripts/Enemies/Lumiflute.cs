using UnityEngine;

public class Lumiflute : EnemyBase
{
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] AudioClip _deathSound;

    private void Awake()
    {
        base.deathSound = _deathSound;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move() {
        transform.Translate(Vector2.left * moveSpeed * Time.fixedDeltaTime);
    }
}
