using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;

    [Header("Jumping")]
    BoxCollider2D groundChecker;

    [SerializeField] int maxJumpCount = 1;
    int jumpCount = 0;

    Vector2 moveInput;
    Rigidbody2D rb;
    BoxCollider2D boxCollider;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        GetGroundChecker();
    }

    void Update()
    {
        Run();
        FlipSprite();

        CheckBottomBoundary();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        //If the player is not touching the ground, don't jump
        //(will change later on to allow for double jumps)
        if (!groundChecker.IsTouchingLayers(LayerMask.GetMask("Jumpable"))){ return; }

        if (value.isPressed)
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

    void CheckBottomBoundary()
    {
        if (groundChecker.IsTouchingLayers(LayerMask.GetMask("BottomBoundary"))) 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}