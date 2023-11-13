using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{       
    PlayerController player;
    CameraController cam;
    GameManager gameManager;
    public bool checkpointReached;
    Transform checkpoint;

    [Header("Level Timer")]
    [SerializeField] float levelTimeout = 10f;
    public float levelTimer { get; private set; }

    // Stamina & Size Shifting
    public float currentEnergy { get; private set; }

    [Header("Stamina & Size Shifting")]

    public float maxEnergy;
    [SerializeField] float startingEnergy;

    [Tooltip("In percent.")] public float pixieEnergyThreshold;
    [Tooltip("In percent.")] public float titanEnergyThreshold;

    [Tooltip("In percent.")] public float nomadEnergyCost, pixieEnergyCost, titanEnergyCost;

    [Header("Game Over")]
    [SerializeField] float returnToMenuDelay = 2f;
    [SerializeField] float screenFadeDuration = 2f;

    void Awake()
    {
        AssignLevelManager();

        player = FindObjectOfType<PlayerController>();
        cam = FindObjectOfType<CameraController>();
        checkpoint = GameObject.FindGameObjectWithTag("Checkpoint").transform;
    }

    void Start()
    {
        currentEnergy = startingEnergy;
        levelTimer = levelTimeout;
    }

    void Update()
    {
        LevelTimer();
    }


    // Level Timer
    void LevelTimer()
    {
        if (levelTimer <= 0f)
        {
            StartCoroutine(LoadLevelsMap());
            return;
        }

        levelTimer -= Time.deltaTime;
    }

    IEnumerator LoadLevelsMap()
    {
        UIManager.Instance.ShowTimerPopup();

        yield return new WaitForSecondsRealtime(returnToMenuDelay);

        yield return UIManager.Instance.SceneFadeIn(screenFadeDuration);

        // Disable the overlay
        StartCoroutine(UIManager.Instance.SceneFadeOut(0f));

        UIManager.Instance.HideTimerPopup();
        GameManager.Instance.LoadLevelsMap();
    }

    // Checkpoint
    public void RespawnOnCheckpoint()
    {
        if (checkpoint != null)
        {
            player.transform.position = checkpoint.position;
            cam.transform.position = checkpoint.position + new Vector3(0f, 0f, -10f);

            currentEnergy = 0;

            levelTimer = levelTimeout;

            Debug.Log("Respawned on checkpoint");
        }
        else
        {
            Debug.Log("Have not reached a checkpoint yet");
        }      
    }

    // Stamina
    public void AddEnergy(float energy)
    {
        currentEnergy += energy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
    }

    public void AddEnergyPercent(float percent)
    {
        float energy = maxEnergy * (percent / 100f);

        currentEnergy += energy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
    }

    void AssignLevelManager()
    {
        gameManager = GameManager.Instance;

        if (gameManager)
        {
            gameManager.levelManager = this;
        }
        else
        {
            Debug.LogError("No GameManager found in the scene");
        }
    }
}