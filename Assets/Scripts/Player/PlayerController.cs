using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    //================== Physics and Game Components ==================
    Rigidbody2D rb;
    LevelManager levelManager;

    //================== Movement Variables ==================
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;
    Vector2 moveInput;
    float currentSpeed, currentJumpForce;

    //================== Jumping Variables ==================  
    [Header("Jumping")]
    [SerializeField] 
    [Tooltip("The time for the last chance to jump after falling off a platform.")]
    float timeToLastChanceJump = 0.5f;
    private float originalJumpDelay;

    [SerializeField]
    [Tooltip("The force applied when jumping in the air after falling off a platform.")]
    float jumpAirForce = 1.2f;

    int maxJumpCount = 1;
    int jumpCount = 0;
    BoxCollider2D groundChecker;

    //================== Shape Shifting Components ==================  

    enum PlayerForm { Pixie, Nomad, Titan }
    PlayerForm currentForm = PlayerForm.Nomad;

    Dictionary<PlayerForm, Vector2> formScaleMapping = new Dictionary<PlayerForm, Vector2>
    {
        { PlayerForm.Titan, new Vector2(0.3f, 0.3f) },
        { PlayerForm.Nomad, new Vector2(0.2f, 0.2f) },
        { PlayerForm.Pixie, new Vector2(0.1f, 0.1f) }
    };

    // Entity
    public float health { get; set; } = 1f;
    public bool hasDied { get; set; } = false;

    //TODO: Transfer to the audio manager
    [Header("Audio")]
    [SerializeField] AudioClip gameOverSound;
    [SerializeField] AudioClip jumpingSound, landingSound;
    [SerializeField] AudioClip pixieSound, nomadSound, titanSound;
    [SerializeField] AudioClip checkpointSound;

    [Header("Other")]
    [SerializeField] bool hasArmor = false;
    bool fellDown = false;

    float baseGravity;
    float currentGravity;
    float gravityAdjustmentSpeed = 2.0f;

    //Momentum
    [SerializeField] float acceleration = 5f;
    [SerializeField] float deceleration = 5f;

    //Animation
    PlayerAnimations playerAnimations;
    bool inputKeyPressed;
    bool jumping;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetGroundChecker();
        playerAnimations = GetComponent<PlayerAnimations>();
        originalJumpDelay = timeToLastChanceJump;
    }

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.levelManager != null)
        {
            levelManager = GameManager.Instance.levelManager;
        }
        else
        {
            Debug.LogError("GameManager or LevelManager is null.");
        }   

        OnNomad();

        currentSpeed = moveSpeed;
        currentJumpForce = jumpForce;
        baseGravity = rb.gravityScale;
        currentGravity = baseGravity;
    }

    void Update()
    {
        Run();
        Jump();
        FlipSprite();
        MonitorGroundedState();
        CheckBottomBoundary();
    }

    #region Movement

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        
        if (IsGrounded())
        {
            playerAnimations.StartAnimation("Run", true, 2f);
            inputKeyPressed = true;
        }
    }

    void Run()
    {
        float targetVelocityX = moveInput.x * moveSpeed;

        // Apply acceleration
        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetVelocityX, acceleration * Time.deltaTime), rb.velocity.y);


        if (Mathf.Approximately(moveInput.x, 0f))
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0f, deceleration * Time.deltaTime), rb.velocity.y);

            if (inputKeyPressed && IsGrounded())
            {
                inputKeyPressed = false;
                playerAnimations.StartAnimation("Idle", true, 1f);
            }
        }
    }

    #endregion

    #region Jumping
    void FlipSprite()
    {
        if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }

    void OnJump()
    {
        if (currentForm != PlayerForm.Titan && jumpCount < maxJumpCount && timeToLastChanceJump > 0f)
        {
            jumping = true;
        }

    }

    void Jump()
    {
        if (jumping && jumpCount < maxJumpCount)
        {
            jumping = false;
            playerAnimations.StartAnimation("Jump", false, 1f);

            if (IsGrounded())
            {
                rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(new Vector2(0f, jumpForce * jumpAirForce), ForceMode2D.Impulse);
            }

            jumpCount++;
            AudioManager.Instance.PlayOneShot(jumpingSound, 0.5f);
        }
    }

    bool IsGrounded()
    {
        return groundChecker.IsTouchingLayers(LayerMask.GetMask("Jumpable")) && rb.velocity.y <= 0f;
    }

    // Updates the player's grounded status and manages the time window for last-chance jumps when falling.
    void MonitorGroundedState()
    {
        if (IsGrounded())
        {
            jumpCount = 0;
            timeToLastChanceJump = originalJumpDelay;
        }
        else
        {
            if (timeToLastChanceJump > 0f)
            {
                timeToLastChanceJump -= Time.deltaTime;
            }
        }
    }

    #endregion

    void GetGroundChecker()
    {
        Transform firstChild = transform.GetChild(0); // Get the first child
        Transform groundCheckerTransform = firstChild.Find("GroundChecker");

        if (groundCheckerTransform != null)
            groundChecker = groundCheckerTransform.GetComponent<BoxCollider2D>();
        else
            Debug.LogError("GroundChecker not found!");
    }

    void CheckBottomBoundary()
    {
        if (groundChecker.IsTouchingLayers(LayerMask.GetMask("BottomBoundary")))
        {
            fellDown = true;
            groundChecker.enabled = false;
            OnDeath();
        }
    }

    #region Size Shifting

    void ChangeForm
        //Parameters
        (   
            PlayerForm targetForm, 
            float energyCost,
            int maxJumps,
            AudioClip transformationSound,
            float energyThreshold = 0f
        )
    {
        if (currentForm != targetForm && levelManager.currentEnergy >= levelManager.maxEnergy * (energyThreshold / 100f))
        {
            levelManager.AddEnergyPercent(-energyCost);
            currentForm = targetForm;

            // Change scale of the player based on the form
            if (formScaleMapping.TryGetValue(targetForm, out Vector2 targetScale))
            {
                transform.GetChild(0).transform.localScale = targetScale;
            }

            AudioManager.Instance.PlayOneShot(transformationSound, 0.8f);

            maxJumpCount = maxJumps;

            if (jumpCount > 0 && targetForm == PlayerForm.Titan)
            {
                playerAnimations.StartAnimation("GroundPound", false, 1f);
            }
            else
            {
                // TODO: Handle back to regular animation
            }
        }
    }

    void OnTitan()
    {
        ChangeForm(PlayerForm.Titan, levelManager.titanEnergyCost, 0, titanSound, levelManager.titanEnergyThreshold);
    }

    void OnNomad()
    {
        ChangeForm(PlayerForm.Nomad, levelManager.nomadEnergyCost, 1, nomadSound);
    }

    void OnPixie()
    {
        ChangeForm(PlayerForm.Pixie, levelManager.pixieEnergyCost, 2, pixieSound, levelManager.pixieEnergyThreshold);
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string animationName = moveInput.x == 0 ? "Idle" : "Run";
        float animationSpeed = moveInput.x == 0 ? 1f : 2f;
        playerAnimations.StartAnimation(animationName, true, animationSpeed);

        if (collision.gameObject.CompareTag("Breakable") && currentForm == PlayerForm.Titan)
        {
            Destroy(collision.gameObject);
            return; // Early exit
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Jumpable"))
        {
            float randomPitch = Random.Range(0.75f, 1.25f);
            AudioManager.Instance.PlayOneShot(landingSound, 0.5f, randomPitch);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectible"))
        {
            ICollectible collectible = collision.gameObject.GetComponent<ICollectible>();
            collectible.OnCollect();
        }

        SetCheckpoint(collision.gameObject);
    }

    void SetCheckpoint(GameObject checkpoint)
    {
        if (checkpoint.CompareTag("Checkpoint") && !levelManager.checkpointReached)
        {
            levelManager.checkpointReached = true;

            SpriteRenderer checkpointRenderer = checkpoint.GetComponent<SpriteRenderer>();
            checkpointRenderer.material.color = Color.green;

            AudioManager.Instance.PlayOneShot(checkpointSound);
        }
    }

    public void Hurt()
    {
        if (hasArmor || currentForm == PlayerForm.Titan)
        {
            hasArmor = false;
            OnNomad();
        }
        else
        {
            OnDeath();            
        }
    }

    public void OnDeath()
    {
        if (!hasDied)
        {
            if (fellDown || (!hasArmor && currentForm != PlayerForm.Titan))
            {
                UnityEvent reset = new UnityEvent();

                // Reset everything back to normal after the respawning is complete
                reset.AddListener(() => groundChecker.enabled = true);
                reset.AddListener(() => rb.velocity = Vector3.zero);

                GameManager.Instance.RestartLevel(reset);
                fellDown = false;

                AudioManager.Instance.PlayOneShot(gameOverSound);
            }
            else
            {
                Debug.Log("Player either has armor or is Titan. Can't kill!");
            }
        }       
    }
}