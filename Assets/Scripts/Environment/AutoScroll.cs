using UnityEngine;

public class AutoScroll : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    GameManager gameManager;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void FixedUpdate()
    {
        if (gameManager != null && gameManager.currentSceneIndex != 0)
        {
            transform.position += transform.right * speed * Time.fixedDeltaTime;
        }
    }
}