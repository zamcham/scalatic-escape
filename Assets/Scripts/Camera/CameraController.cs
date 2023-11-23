using UnityEngine;

public class CameraController : MonoBehaviour
{
#region Singleton Declaration
    public static CameraController Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
        
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject);
            return;
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    #endregion

    //Serialized Values
    [Header("General")]
    [SerializeField] float speed = 10f;

    [SerializeField] Transform leftBorder;
    [SerializeField] float playerKillDistance = 3f;
    [Space]
    [Header("Camera Arm Settings")]
    [SerializeField] private Transform _ArmTransform;
    [SerializeField] private Vector3 _ArmLimits;
    [SerializeField] private Transform _ArmTarget;

    public float _speedModifier = 1;

    // Private Variables
    GameManager gameManager;
    PlayerController player;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        //UpdateCameraArmPosition();
        CheckPlayer();
    }

    void FixedUpdate()
    {
        if (gameManager.currentSceneIndex != 0)
        {
            this.transform.position += transform.right * (speed * _speedModifier) * Time.fixedDeltaTime;
        }
    }

    void CheckPlayer()
    {
        if (leftBorder.position.x - playerKillDistance > player.transform.position.x)
        {
            player.Hurt(true);
        }
    }
 
    private void UpdateCameraArmPosition()
    {
        float targetY = _ArmTarget.transform.position.y;
        if(_ArmLimits.y < Mathf.Abs(targetY)) {
            targetY = _ArmLimits.y * Mathf.Sign(targetY) ;
        }

        Vector3 newLocalPosition = new Vector3(0,0,0);
        newLocalPosition.y = targetY;

        _ArmTransform.localPosition = newLocalPosition;
    }

    
    public void SetSpeedModifier(float newSpeedModifier)
    {
        _speedModifier = newSpeedModifier;
    }
}