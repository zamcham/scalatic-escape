using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float jumpForce = 10f;

    [Header("Jumping")]
    BoxCollider2D groundChecker;

    [SerializeField] int maxJumpCount = 1;
    int jumpCount = 0;

    Vector2 moveInput;
    Rigidbody2D rb;

    [Header("Size Shifting & Energy")]
    EnergyManagement energyManagement;
    GameObject pixie, nomad, titan;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        energyManagement = GetComponent<EnergyManagement>();

        GetGroundChecker();
        GetCharacters();
    }

    void Update()
    {
        if (GameManager.Instance.isPaused) { return; }

        Run();
        FlipSprite();

        CheckBottomBoundary();
    }

    #region Movement
    void OnMove(InputValue value)
    {
        if (GameManager.Instance.isPaused) { return; }

        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (GameManager.Instance.isPaused) { return; }

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
            GameManager.Instance.ReloadScene();
        }
    }
    #endregion

    #region Size Shifting

    void OnTitan(InputValue value)
    {
        if (GameManager.Instance.isPaused) { return; }

        if (!titan.activeSelf)
        {
            if (energyManagement.currentStamina >= energyManagement.maxStamina * (energyManagement.titanEnergyThreshold / 100f))
            {
                energyManagement.AddStaminaPercent(-energyManagement.titanEnergyCost);

                pixie.SetActive(false);
                nomad.SetActive(false);

                titan.SetActive(true);

                maxJumpCount = 0;

                Debug.Log("Changed to Titan.");
            }
            else
            {
                Debug.Log("Not enough energy left for Titan.");
            }
        }
        else
        {
            Debug.Log("Titan is already active.");
        }
    }

    void OnNomad(InputValue value)
    {
        if (GameManager.Instance.isPaused) { return; }

        if (!nomad.activeSelf)
        {
            energyManagement.AddStaminaPercent(-energyManagement.nomadEnergyCost);

            pixie.SetActive(false);
            titan.SetActive(false);

            nomad.SetActive(true);

            maxJumpCount = 1;

            Debug.Log("Changed to Nomad.");
        }
        else
        {
            Debug.Log("Nomad is already active.");
        }
    }

    void OnPixie(InputValue value)
    {
        if (GameManager.Instance.isPaused) { return; }

        if (!pixie.activeSelf)
        {
            if (energyManagement.currentStamina >= energyManagement.maxStamina * (energyManagement.pixieEnergyThreshold / 100f))
            {
                energyManagement.AddStaminaPercent(-energyManagement.pixieEnergyCost);

                titan.SetActive(false);
                nomad.SetActive(false);

                pixie.SetActive(true);

                maxJumpCount = 2;

                Debug.Log("Changed to Pixie.");
            }
            else
            {
                Debug.Log("Not enough energy left for Pixie.");
            }
        }
        else
        {
            Debug.Log("Pixie is already active.");
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
                        break;
                }
            }
        }
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectible"))
        {
            ICollectible collectible = collision.gameObject.GetComponent<ICollectible>();
            collectible.OnCollect(gameObject);
        }
    }
}