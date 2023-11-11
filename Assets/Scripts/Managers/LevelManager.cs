using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{    
    public static LevelManager Instance { get; private set; }
    GameManager gameManager;

    // Checkpoint
    PlayerController player;
    CameraController cam;
    public bool checkpointReached;
    Transform checkpoint;

    // Stamina & Size Shifting
    public float currentEnergy { get; private set; }

    [Header("Stamina & Size Shifting")]

    public float maxEnergy;
    [SerializeField] float startingEnergy;

    [Tooltip("In percent.")] public float pixieEnergyThreshold;
    [Tooltip("In percent.")] public float titanEnergyThreshold;

    [Tooltip("In percent.")] public float nomadEnergyCost, pixieEnergyCost, titanEnergyCost;

    [Header("UI")]
    [SerializeField] BarUI energyBar;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        gameManager = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerController>();
        cam = FindObjectOfType<CameraController>();
        checkpoint = GameObject.FindGameObjectWithTag("Checkpoint").transform;

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentEnergy = startingEnergy;
        energyBar.SetValue(currentEnergy);
    }

    void Update()
    {
        if (onCheckpoint)
        {
            currentCheckpoint = new Checkpoint(player.transform.position, cam.transform.position, currentEnergy, RetrieveEntities());
            onCheckpoint = false;
        }

        // Quick testing
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RespawnOnCheckpoint();
        }
    }

    // Checkpoint system
    Transform[] RetrieveEntities()
    {
        List<Transform> entities = new List<Transform>();

        foreach (var item in GameObject.FindGameObjectsWithTag("SceneEntity"))
        {
            entities.Add(item.transform);
        }

        return entities.ToArray();
    }

    public void RespawnOnCheckpoint()
    {
        if (currentCheckpoint != null)
        {
            player.transform.position = currentCheckpoint.playerPosition;
            cam.transform.position = currentCheckpoint.cameraPosition;

            currentEnergy = currentCheckpoint.energy;

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

        energyBar.SetValue(currentEnergy);
    }

    public void AddEnergyPercent(float percent)
    {
        float energy = maxEnergy * (percent / 100f);

        currentEnergy += energy;
        currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);

        energyBar.SetValue(currentEnergy);
    }
}