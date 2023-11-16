using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Spore : MonoBehaviour
{
    public enum SporeMode { Trap, Air }

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(SporeMode mode, float force)
    {
        switch (mode)
        {
            case SporeMode.Trap:
                rb.gravityScale = 0.1f;

                break;

            case SporeMode.Air:
                rb.gravityScale = 0f;
                break;
        }

        rb.AddForce(transform.parent.up * force);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.root.CompareTag("Player"))
        {
            collision.transform.root.GetComponent<PlayerController>().Hurt();
        }
    }
}