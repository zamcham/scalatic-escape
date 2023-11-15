using System.Collections;
using UnityEngine;

public class RollingEnemy : EnemyBase
{
    [SerializeField] Transform sprite;
    [SerializeField] float moveSpeed = 2f, rollDegreePerUnit = 90;

    // Corner Detection
    [SerializeField] float rayMaxDistance = 0.5f, changeDirectionDuration = 0.5f;
    [SerializeField] LayerMask rayMask;

    float currentSpeed, currentDirection;

    bool changingDirection = false;

    private void Start()
    {
        currentDirection = 1f;
        currentSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        if (!changingDirection && 
            (Physics2D.Raycast(transform.position, transform.right, rayMaxDistance, rayMask) ||
            Physics2D.Raycast(transform.position, -transform.right, rayMaxDistance, rayMask)))
        {
            StartCoroutine(ChangeDirection(-currentDirection, changeDirectionDuration, 2f));
        }

        Move();
        Roll();
    }

    IEnumerator ChangeDirection(float direction, float duration, float delayBeforeEnd)
    {
        changingDirection = true;

        float lerp = 0f;
        float initialDirection = currentDirection;

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / duration;

            currentDirection = Mathf.Lerp(initialDirection, direction, lerp);

            yield return null;
        }

        // To ensure we are away from the corner
        yield return new WaitForSeconds(delayBeforeEnd);

        changingDirection = false;
    }

    void Move()
    {
        transform.Translate(Vector2.left * (currentSpeed * currentDirection) * Time.fixedDeltaTime);
    }

    void Roll()
    {
        sprite.Rotate(Vector3.forward * ((currentSpeed * currentDirection) * rollDegreePerUnit) * Time.fixedDeltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + (transform.right * rayMaxDistance));
        Gizmos.DrawLine(transform.position, transform.position + (-transform.right * rayMaxDistance));
    }
}