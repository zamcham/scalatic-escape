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

    GameObject pixie, nomad, titan;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GetGroundChecker();
        GetCharacters();
    }

    void Update()
    {
        Run();
        FlipSprite();

        CheckBottomBoundary();
    }

    #region Movement
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log(moveInput);
    }

    void OnJump(InputValue value)
    {
        CheckGround();

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
    void CheckGround()
    {
        if (groundChecker.IsTouchingLayers(LayerMask.GetMask("Jumpable")) && rb.velocity.y <= 0f)
        {
            jumpCount = 0;
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
    #endregion

    #region Size Shifting

    void OnTitan(InputValue value)
    {
        Debug.Log("Titan");

        titan.SetActive(true);

        pixie.SetActive(false);
        nomad.SetActive(false);

        maxJumpCount = 0;
    }

    void OnNomad(InputValue value)
    {
        Debug.Log("Nomad");

        nomad.SetActive(true);

        pixie.SetActive(false);
        titan.SetActive(false);

        maxJumpCount = 1;
    }

    void OnPixie(InputValue value)
    {
        Debug.Log("Pixie");

        pixie.SetActive(true);

        titan.SetActive(false);
        nomad.SetActive(false);

        maxJumpCount = 2;
    }

    void GetCharacters()
    {
        Transform pixie = transform.Find("Pixie");
        Transform nomad = transform.Find("Nomad");
        Transform titan = transform.Find("Titan");

        if (pixie == null)
        {
            Debug.LogError("Pixie mesh not found!");
        }
        else
        {
            this.pixie = pixie.gameObject;
        }

        if (nomad == null)
        {
            Debug.LogError("Nomad mesh not found!");
        }
        else
        {
            this.nomad = nomad.gameObject;         
        }

        if (titan == null)
        {
            Debug.LogError("Titan mesh not found!");
        }
        else
        {
            this.titan = titan.gameObject;
        }
    }

    #endregion
}