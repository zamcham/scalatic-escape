using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;

    [Header("Jumping")]
    [SerializeField] private TriggerCollision jumpDetector;

    // Making it public so we can dynamically change it later on
    public int maxJumpCount = 1;
    private int jumpCount = 0;

    Vector2 moveInput;
    Rigidbody2D rb;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Run();
        FlipSprite();

        // Ground check
        if (jumpDetector.isHit && rb.velocity.y <= 0f)
        {
            jumpCount = 0;
        }
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed && jumpCount < maxJumpCount)
        {
            rb.velocity = new Vector2(rb.velocity.y, jumpForce);
            jumpCount++;
        }
    }

    void Run()
    {
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
        }        
    }
}