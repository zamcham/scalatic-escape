using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{    
    PlayerController player;
    CameraController cam;
    GameManager gameManager;
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
        AssignLevelManager();

        player = FindObjectOfType<PlayerController>();
        cam = FindObjectOfType<CameraController>();
        checkpoint = GameObject.FindGameObjectWithTag("Checkpoint").transform;
    }

    void Start()
    {
        currentEnergy = startingEnergy;
        energyBar.SetValue(currentEnergy);
    }

    void Update()
    {

    }

    public void RespawnOnCheckpoint()
    {
        if (checkpoint != null)
        {
            player.transform.position = checkpoint.position;
            cam.transform.position = checkpoint.position + new Vector3(0f, 0f, -10f);

            currentEnergy = 0;

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