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

    //================== Speed Boost Components ==================
    [Header("Speed Boost")]
    [SerializeField] float nomadSpeedBoostMultiplier;
    [SerializeField] float nomadJumpBoostMultiplier;

    [SerializeField] float pixieSpeedBoostMultiplier;
    [SerializeField] float pixieJumpBoostMultiplier;

    [SerializeField] float speedBoostDuration, speedBoostCooldown;

    float boostDuration, boostCooldown;

    // Pixie speed combo
    int pixieCurrentCombo = 0;
    float pixieComboCooldown = 0.5f, pixieComboTimer = 0f;

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
        PixieComboUpdate();
        SpeedBoostUpdate();

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
                Debug.Log("switching to Idle");
                playerAnimations.StartAnimation("Idle", true, 1f);
            }
        }

    }

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

    void CheckGround()
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

    void GroundPounding(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Breakable"))
        {
            Destroy(collision.gameObject);
        }
        else {
            playerAnimations.StartAnimation("Idle", true, 1f);
        }
    }

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

    #endregion

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
        GroundPounding(collision);

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

    #region Speed Boost

    void SpeedBoostUpdate()
    {
        if (boostCooldown <= 0f)
        {
            if (boostDuration > 0f)
            {
                boostDuration -= Time.deltaTime;
            }
            else if (boostDuration > -1f)
            {
                boostDuration = -1f;
                boostCooldown = speedBoostCooldown;

                currentSpeed = moveSpeed;
                currentJumpForce = jumpForce;

                Debug.Log("Speed boost is done.");
            }
        }
        else
        {
            boostCooldown -= Time.deltaTime;          
        }
    }

    void OnNomadSpeedBoost()
    {
        if (boostCooldown > 0f)
        {
            Debug.Log("Nomad speed boost couldn't be activated! Cooldown is ongoing.");
        }
        else if (currentForm == PlayerForm.Nomad && boostDuration <= 0f)
        {
            Debug.Log("Nomad speed boost is  activated!");

            currentSpeed = moveSpeed * nomadSpeedBoostMultiplier;
            currentJumpForce = jumpForce * nomadJumpBoostMultiplier;

            boostDuration = speedBoostDuration;
        }
    }

    void PixieComboUpdate()
    {
        if (pixieComboTimer >= 0f)
        {
            pixieComboTimer -= Time.deltaTime;
        }
        else if (pixieComboTimer > -1f)
        {
            pixieCurrentCombo = 0;
            pixieComboTimer = -1f;
        }
    }

    void OnPixieSpeedBoostCombo1()
    {
        if (pixieCurrentCombo == 0)
        {
            pixieCurrentCombo = 1;
            pixieComboTimer = pixieComboCooldown;
        }
    }

    void OnPixieSpeedBoostCombo2()
    {
        if (pixieCurrentCombo == 1)
        {
            pixieCurrentCombo = 2;
            pixieComboTimer = pixieComboCooldown;
        }
    }

    void OnPixieSpeedBoostCombo3()
    {
        if (currentForm == PlayerForm.Pixie && pixieCurrentCombo == 2)
        {
            if (boostCooldown > 0f)
            {
                Debug.Log("Pixie speed boost couldn't be activated! Cooldown is ongoing.");
            }
            else if(boostDuration <= 0f)
            {
                Debug.Log("Pixie speed boost is  activated!");

                currentSpeed = moveSpeed * pixieSpeedBoostMultiplier;
                currentJumpForce = jumpForce * pixieJumpBoostMultiplier;

                boostDuration = speedBoostDuration;
            }            
        }

        pixieComboTimer = 0f;
    }

    #endregion
}