using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;

    [Header("Jumping")]
    [SerializeField] private TriggerCollision jumpDetector;
    BoxCollider2D groundChecker;

    // Making it public so we can dynamically change it later on
    public int maxJumpCount = 1;
    private int jumpCount = 0;

    Vector2 moveInput;
    Rigidbody2D rb;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetGroundChecker();
    }

    void Update()
    {
        Run();
        FlipSprite();
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

    void GetGroundChecker()
    {
        Transform groundCheckerTransform = transform.Find("GroundChecker");

        if (groundCheckerTransform == null)
        {
            Debug.LogError("GroundChecker not found!");
        }
        else
        {
            groundChecker = groundCheckerTransform.gameObject.GetComponent<BoxCollider2D>();
        }
    }
}