using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    public bool sceneLoading { get; private set; } = false;
    public bool respawning { get; private set; } = false;

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
        if (!sceneLoading)
        {
            if (levelTimer <= 0f)
            {
                UnityEvent preparation = new UnityEvent(), reset = new UnityEvent();

                preparation.AddListener(() => UIManager.Instance.ShowTimerPopup());
                reset.AddListener(() => UIManager.Instance.HideTimerPopup());

                LoadLevelSelection(preparation, reset);
                return;
            }

            levelTimer -= Time.deltaTime;
        }       
    }

    public void ReloadLevel()
    {
        // Start the coroutine in GameManager so it continues when this script is destroyed during level transiion
        GameManager.Instance.StartCoroutine(ReloadLevelIE(null, null));
    }

    IEnumerator ReloadLevelIE(UnityEvent preparationEvent = null, UnityEvent resetEvent = null)
    {
        // For readability
        UIManager UIinstance = UIManager.Instance;
        GameManager gameInstance = GameManager.Instance;

        // Internal preparation
        sceneLoading = true;
        Time.timeScale = 0f; // Freeze the game

        // Transition preperation 
        if (preparationEvent != null)
        {
            preparationEvent.Invoke();
        }

        // First, wait for the screen fade-in to start. Then wait for it to end.
        yield return new WaitForSecondsRealtime(returnToMenuDelay);
        yield return UIinstance.SceneFadeIn(screenFadeDuration);

        // Fade-In is done. Load the scene asynchronously and wait until it's done
        AsyncOperation asyncLoad = gameInstance.ReloadLevelAsync();
        yield return new WaitUntil(() => asyncLoad.isDone);

        // The scene is fully loaded. Reset everything.
        if (resetEvent != null)
        {
            resetEvent.Invoke();
        }

        // Internal reset. This must be done in all conditions.
        yield return UIinstance.StartCoroutine(UIinstance.SceneFadeOut(screenFadeDuration));
        Time.timeScale = 1f;
        sceneLoading = false;
    }

    public void LoadLevelSelection(UnityEvent preparationEvent, UnityEvent resetEvent)
    {
        // Start the coroutine in GameManager so it continues when this script is destroyed during level transiion
        GameManager.Instance.StartCoroutine(LoadLevelsMap(preparationEvent, resetEvent));
    }
    IEnumerator LoadLevelsMap(UnityEvent preparationEvent, UnityEvent resetEvent)
    {        
        // For readability
        UIManager UIinstance = UIManager.Instance;
        GameManager gameInstance = GameManager.Instance;

        // Internal preparation
        sceneLoading = true;
        Time.timeScale = 0f; // Freeze the game

        // Transition preperation 
        preparationEvent.Invoke();

        // First, wait for the screen fade-in to start. Then wait for it to end.
        yield return new WaitForSecondsRealtime(returnToMenuDelay);
        yield return UIinstance.SceneFadeIn(screenFadeDuration);

        // Fade-In is done. Load the scene asynchronously and wait until it's done
        AsyncOperation asyncLoad = gameInstance.LoadLevelsMapAsync();
        yield return new WaitUntil(() => asyncLoad.isDone);

        // The scene is fully loaded. Reset everything.
        resetEvent.Invoke();

        // Internal reset. This must be done in all conditions.
        UIinstance.StartCoroutine(UIinstance.SceneFadeOut(screenFadeDuration));
        Time.timeScale = 1f;
        sceneLoading = false;
    }

    // Checkpoint
    public void RespawnOnCheckpoint()
    {
        if (!respawning)
        {
            StartCoroutine(OnCheckpoint());      
        }
    }

    IEnumerator OnCheckpoint()
    {
        if (checkpoint != null)
        {
            respawning = true;

            // For readability
            UIManager UIinstance = UIManager.Instance;

            Time.timeScale = 0f; // Freeze the game

            // First, wait for the screen fade-in to start. Then wait for it to end.
            yield return new WaitForSecondsRealtime(returnToMenuDelay);
            yield return UIinstance.SceneFadeIn(screenFadeDuration);

            // Fade-In is done. Respawn from checkpoint
            player.transform.position = checkpoint.position;
            cam.transform.position = checkpoint.position + new Vector3(0f, 0f, -10f);

            currentEnergy = 0;
            levelTimer = levelTimeout;

            // Fade out
            yield return UIinstance.SceneFadeOut(screenFadeDuration);
            Time.timeScale = 1f;            

            Debug.Log("Respawned on checkpoint");

            // This is a bit tricky issue to explain but the respawn method will run forever if I don't add this
            for (int i = 0; i < 5; i++)
            {
                yield return null;
            }

            respawning = false;
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