using UnityEngine;

[DisallowMultipleComponent]
public class LevelManager : MonoBehaviour
{    
    public static LevelManager Instance { get; private set; }
    GameManager gameManager;

    // Stamina & Size Shifting
    public float currentEnergy { get; private set; }
    public float maxEnergy;

    [Header("Stamina & Size Shifting")]

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
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        currentEnergy = startingEnergy;
        energyBar.SetValue(currentEnergy);
    }

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