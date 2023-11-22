using System.Collections;
using UnityEngine;

public class RollingEnemy : EnemyBase
{
    [SerializeField] Transform sprite;
    [SerializeField] float moveSpeed = 2f, rollDegreePerUnit = 90;

    // Corner Detection
    [SerializeField] new Collider2D collider;

    [SerializeField] float horizontalRayDistance = 0.5f, verticalRayDistance = 0.5f, changeDirectionDuration = 0.5f;
    [SerializeField] LayerMask rayMask;

    [SerializeField] AudioClip _deathSound;

    float currentSpeed, currentDirection;

    bool changingDirection = false;

    Vector3 right, left, rightBottom, leftBottom;

    private void Start()
    {
        base.deathSound = _deathSound;

        currentDirection = 1f;
        currentSpeed = moveSpeed;
    }

    private void FixedUpdate()
    {
        if (!changingDirection)
        {
            // This operation is heavy so only calculate it here

            // For right - left raycasts
            right = collider.bounds.center + (collider.bounds.extents.x * Vector3.right);
            left = collider.bounds.center - (collider.bounds.extents.x * Vector3.right);

            // For bottom raycasts
            rightBottom = collider.bounds.center + new Vector3(collider.bounds.extents.x, -collider.bounds.extents.y, 0f);
            leftBottom = collider.bounds.center - collider.bounds.extents;

            bool horizontalRaycasts = Physics2D.Raycast(right, transform.right, horizontalRayDistance, rayMask) // If there is an obstacle to our right
            || Physics2D.Raycast(left, -transform.right, horizontalRayDistance, rayMask); // Same but left

            if (horizontalRaycasts)
            {
                StartCoroutine(ChangeDirection(-currentDirection, changeDirectionDuration, 1f));
            }
            else
            {
                // So, there is a thing. These raycasts are so accurate that, even if there is a minimal space between two ground objects, 
                // the enemy will change it direction. Of course this is an undesired behavior. If the space is minimal, the enemy can roll easily.
                // So, we will consider the half of its size here. Sorry for the spaghetti code lol.

                bool bottomRightRaycast = !Physics2D.Raycast(rightBottom, -transform.up, verticalRayDistance, rayMask);
                bool bottomLeftRaycast =  !Physics2D.Raycast(leftBottom, -transform.up, verticalRayDistance, rayMask);

                bool sizeRaycast = false;

                Vector3 extentsHorizontal = new Vector3(collider.bounds.extents.x, 0f, 0f);

                if (bottomRightRaycast)
                {
                    sizeRaycast = !Physics2D.Raycast(rightBottom + extentsHorizontal, -transform.up, verticalRayDistance, rayMask);
                }
                else if (bottomLeftRaycast)
                {
                    sizeRaycast = !Physics2D.Raycast(leftBottom - extentsHorizontal, -transform.up, verticalRayDistance, rayMask);
                }

                if (sizeRaycast)
                {
                    StartCoroutine(ChangeDirection(-currentDirection, changeDirectionDuration, 1f));
                }
            }           
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
        Gizmos.DrawLine(right, right + (transform.right * horizontalRayDistance));
        Gizmos.DrawLine(left, left - (transform.right * horizontalRayDistance));

        Gizmos.DrawLine(rightBottom, rightBottom - (transform.up * verticalRayDistance));
        Gizmos.DrawLine(leftBottom, leftBottom - (transform.up * verticalRayDistance));
    }
}