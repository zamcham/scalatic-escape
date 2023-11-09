using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;

    public int score { get; private set; } = 10;
    public float stamina { get; set; }
    public float remainingTime {  get; private set; }

    public int collectedPickups { get; private set; }

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            Destroy(gameObject);

            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}