using UnityEngine;

public class GameSession : MonoBehaviour
{
    
    GameManager gameManager;
    // Start is called before the first frame update

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}