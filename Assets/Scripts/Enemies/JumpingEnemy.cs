using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpingEnemy : EnemyBase
{
    Rigidbody2D rb;
    [SerializeField] float jumpForce = 100f, jumpInterval = 3f;

    bool isGrounded;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float timer = 0f;

        while (true)
        {
            if (isGrounded)
            {
                if (timer > jumpInterval)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);

                    timer = 0f;
                }
                else
                {
                    timer += Time.deltaTime;
                } 
            }

            yield return null;
        }
    }

    private void OnCollisionStay2D()
    {
        isGrounded = true;
    }

    private void OnCollisionExit2D()
    {
        isGrounded = false;
    }
}