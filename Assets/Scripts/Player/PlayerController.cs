using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;

    float currentSpeed, currentJumpForce;

    [Header("Jumping")]
    BoxCollider2D groundChecker;
    [SerializeField] float jumpInterval = 0.25f;
    int maxJumpCount = 1;
    int jumpCount = 0, jumpsInQueue = 0;
    float jumpTimer;

    Vector2 moveInput;
    Rigidbody2D rb;

    [Header("Size Shifting & Energy")]
    LevelManager levelManager;
    GameObject pixie, nomad, titan;
    bool canBreakPlatform = false;
    Renderer titanRenderer;

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

    [Header("Audio")]
    [SerializeField] AudioClip gameOverSound;
    [SerializeField] AudioClip jumpingSound, landingSound;
    [SerializeField] AudioClip pixieSound, nomadSound, titanSound;
    [SerializeField] AudioClip checkpointSound;

    [Header("Other")]
    [SerializeField] bool hasArmor = false;
    bool fellDown = false;

    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GetGroundChecker();
        GetCharacters();     
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

        ResetCharacters();
        OnNomad();

        currentSpeed = moveSpeed;
        currentJumpForce = jumpForce;
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
    }

    void Run()
    {
        rb.velocity = new Vector2(moveInput.x * currentSpeed, rb.velocity.y);
    }

    void FlipSprite()
    {
        if (Mathf.Abs(rb.velocity.x) > Mathf.Epsilon)
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f);
    }

    void OnJump()
    {
        if (jumpsInQueue < maxJumpCount)
            jumpsInQueue++;
    }

    void Jump()
    {
        if (jumpsInQueue > 0 && jumpTimer <= 0f)
        {
            if (jumpCount < maxJumpCount) 
            {
                jumpTimer = jumpInterval;
                jumpsInQueue--;
                jumpCount++;
                rb.velocity = new Vector2(rb.velocity.y, currentJumpForce);

                AudioManager.Instance.PlayOneShot(jumpingSound, 0.5f);
            }
        }
        else
        {
            jumpTimer -= Time.deltaTime;
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
            jumpTimer = 0f;
            jumpCount = 0; 
        }
    }

    void GroundPounding(Collision2D collision)
    {
        if (canBreakPlatform && collision.gameObject.CompareTag("Breakable"))
            Destroy(collision.gameObject);
        else if (canBreakPlatform)
        {
            canBreakPlatform = false;
            titanRenderer.sharedMaterial.color = Color.white;
        }
    }

    void GetGroundChecker()
    {
        Transform groundCheckerTransform = transform.Find("GroundChecker");
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

    void OnTitan()
    {
        if (!titan.activeSelf && levelManager.currentEnergy >= levelManager.maxEnergy * (levelManager.titanEnergyThreshold / 100f))
        {
            levelManager.AddEnergyPercent(-levelManager.titanEnergyCost);
            pixie.SetActive(false);
            nomad.SetActive(false);
            titan.SetActive(true);

            AudioManager.Instance.PlayOneShot(titanSound, 0.8f);

            maxJumpCount = 0;

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

    void OnNomad()
    {
        if (!nomad.activeSelf)
        {
            levelManager.AddEnergyPercent(-levelManager.nomadEnergyCost);
            pixie.SetActive(false);
            titan.SetActive(false);
            nomad.SetActive(true);
            maxJumpCount = 1;

            AudioManager.Instance.PlayOneShot(nomadSound, 0.8f);
        }
    }

    void OnPixie()
    {       
        if (!pixie.activeSelf && levelManager.currentEnergy >= levelManager.maxEnergy * (levelManager.pixieEnergyThreshold / 100f))
        {
            levelManager.AddEnergyPercent(-levelManager.pixieEnergyCost);
            titan.SetActive(false);
            nomad.SetActive(false);
            pixie.SetActive(true);
            maxJumpCount = 2;

            AudioManager.Instance.PlayOneShot(pixieSound, 0.8f);
        }
    }

    void GetCharacters()
    {
        string[] forms = { "Pixie", "Nomad", "Titan" };

        foreach (string form in forms)
        {
            Transform currentForm = transform.Find(form);

            if (currentForm == null)
                Debug.LogError($"{form} mesh not found!");
            else
                AssignCharacter(form, currentForm.gameObject);
        }
    }

    void AssignCharacter(string form, GameObject character)
    {
        switch (form)
        {
            case "Pixie":
                pixie = character;
                break;
            case "Nomad":
                nomad = character;
                break;
            case "Titan":
                titan = character;
                titanRenderer = titan.transform.Find("Body").GetComponent<Renderer>();
                break;
        }
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

    void ResetCharacters()
    {
        nomad.SetActive(false);
        titan.SetActive(false);
        pixie.SetActive(false);
    }

    public void Hurt()
    {
        if (hasArmor || titan.activeSelf)
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
            if (fellDown || (!hasArmor && !titan.activeSelf))
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
        else if (nomad.activeSelf && boostDuration <= 0f)
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
        if (pixie.activeSelf && pixieCurrentCombo == 2)
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