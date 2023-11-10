using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;

    [Header("Jumping")]
    BoxCollider2D groundChecker;

    [SerializeField] float jumpInterval = 0.25f;
    int maxJumpCount = 1;
    int jumpCount = 0, jumpsInQueue = 0;
    float jumpTimer;

    Vector2 moveInput;
    Rigidbody2D rb;

    [Header("Size Shifting & Energy")]
    EnergyManagement energyManagement;
    GameObject pixie, nomad, titan;


    // Titan-Specific
    bool canBreakPlatform = false;
    Renderer titanRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        energyManagement = GetComponent<EnergyManagement>();

        GetGroundChecker();
        GetCharacters();        
    }

    void Start()
    {
        // Reset characters
        nomad.SetActive(false);
        titan.SetActive(false);
        pixie.SetActive(false);

        OnNomad();
    }

    void Update()
    {
        Run();
        FlipSprite();

        CheckGround();
        Jump();
        CheckBottomBoundary();
    }

    #region Movement
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
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

    void OnJump()
    {
        // Queue can only be 1 larger than maxJumpCount
        // This is to prevent infinite queues
        if (jumpsInQueue < maxJumpCount)
        {
            jumpsInQueue++;
        }
    }

    void Jump()
    {
        if (jumpsInQueue > 0)
        {
            if (jumpTimer <= 0f)
            {
                if (jumpCount < maxJumpCount) 
                {
                    jumpTimer = jumpInterval;
                    jumpsInQueue--;
                    jumpCount++;

                    rb.velocity = new Vector2(rb.velocity.y, jumpForce);
                }           
            }
            else
            {
                jumpTimer -= Time.deltaTime;
            }
        }
    }


    bool IsGrounded()
    {
        bool grounded = groundChecker.IsTouchingLayers(LayerMask.GetMask("Jumpable")) && rb.velocity.y <= 0f;

        return grounded;
    }
    void CheckGround()
    {
        if (IsGrounded())
        {
            jumpTimer = 0f;
            jumpCount = 0; 
        }
    }

    void GroundPounding(Collision2D collision)
    {
        if (canBreakPlatform)
        {
            if (collision.gameObject.CompareTag("Breakable"))
            {
                Destroy(collision.gameObject);
            }
            else
            {
                canBreakPlatform = false;
                titanRenderer.sharedMaterial.color = Color.white;
            }
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

    void OnTitan()
    {
        if (!titan.activeSelf)
        {
            if (energyManagement.currentEnergy >= energyManagement.maxEnergy * (energyManagement.titanEnergyThreshold / 100f))
            {
                energyManagement.AddEnergyPercent(-energyManagement.titanEnergyCost);

                pixie.SetActive(false);
                nomad.SetActive(false);

                titan.SetActive(true);

                maxJumpCount = 0;

                // Check for ground pounding
                if (jumpCount > 0)
                {
                    canBreakPlatform = true;

                    titanRenderer.sharedMaterial.color = Color.red;
                }
                else
                {
                    titanRenderer.sharedMaterial.color = Color.white;
                }
            }
        }
    }

    void OnNomad()
    {
        if (!nomad.activeSelf)
        {
            energyManagement.AddEnergyPercent(-energyManagement.nomadEnergyCost);

            pixie.SetActive(false);
            titan.SetActive(false);

            nomad.SetActive(true);

            maxJumpCount = 1;
        }
    }

    void OnPixie()
    {       
        if (!pixie.activeSelf)
        {
            if (energyManagement.currentEnergy >= energyManagement.maxEnergy * (energyManagement.pixieEnergyThreshold / 100f))
            {
                energyManagement.AddEnergyPercent(-energyManagement.pixieEnergyCost);

                titan.SetActive(false);
                nomad.SetActive(false);

                pixie.SetActive(true);

                maxJumpCount = 2;
            }
        }
    }

    void GetCharacters()
    {
        string[] forms = { "Pixie", "Nomad", "Titan" };

        foreach (string form in forms)
        {
            Transform currentForm = transform.Find(form);

            if (currentForm == null)
            {
                Debug.LogError($"{form} mesh not found!");
            }
            else
            {
                switch (form)
                {
                    case "Pixie":
                        this.pixie = currentForm.gameObject;
                        break;
                    case "Nomad":
                        this.nomad = currentForm.gameObject;
                        break;
                    case "Titan":
                        this.titan = currentForm.gameObject;

                        titanRenderer = titan.transform.Find("Body").GetComponent<Renderer>();
                        break;
                }
            }
        }
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GroundPounding(collision);     
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectible"))
        {
            ICollectible collectible = collision.gameObject.GetComponent<ICollectible>();
            collectible.OnCollect(gameObject);
        }
    }
}