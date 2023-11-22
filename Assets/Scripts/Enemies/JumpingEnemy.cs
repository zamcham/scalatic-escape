using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class JumpingEnemy : EnemyBase
{
    Rigidbody2D rb;
    [SerializeField] float jumpForce = 100f, jumpInterval = 3f;
    [SerializeField] AudioClip _deathSound, jumpingSound, landingSound;

    bool isGrounded;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float timer = 0f;

        base.deathSound = _deathSound;

        while (true)
        {
            if (isGrounded)
            {
                if (timer > jumpInterval)
                {
                    rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
                    AudioSource.PlayClipAtPoint(jumpingSound, transform.position + (Vector3.forward * -10f), 2f);


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
        if (isGrounded == false)
        {
            // 3D Audio for this
            AudioSource.PlayClipAtPoint(landingSound, transform.position + (Vector3.forward * -10f), 2f);
        }
        isGrounded = true;
    }

    private void OnCollisionExit2D()
    {
        isGrounded = false;
    }
}