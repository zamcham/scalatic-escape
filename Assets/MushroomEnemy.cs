using UnityEngine;

public class MushroomEnemy : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Transform sideSpawnPoints;
    [SerializeField] Transform upperSpawnPoints;

    [Header("Spore Settings")]
    [SerializeField] GameObject sporePrefab;
    [SerializeField] float sporeCreateInterval = 1f;

    // Turn it into a coroutine for now
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}