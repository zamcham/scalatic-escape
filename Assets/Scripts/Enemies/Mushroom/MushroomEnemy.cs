using System.Collections;
using UnityEngine;

public class MushroomEnemy : EnemyBase
{
    [Header("General")]
    [SerializeField] Transform[] sideSpawnPoints;
    [SerializeField] Transform[] upperSpawnPoints;

    [Header("Spore Settings")]
    [SerializeField] GameObject sporePrefab;
    [SerializeField] float sporeCreateInterval = 1f;

    // Turn it into a coroutine for now
    IEnumerator Start()
    {
        float timer = 0f;

        while (true)
        {
            if (timer > sporeCreateInterval)
            {
                CreateSpores();
                timer = 0f;
            }
            else
            {
                timer += Time.deltaTime;
            }

            yield return null;
        }
    }

    void CreateSpores()
    {
        foreach (Transform side in sideSpawnPoints)
        {
            GameObject spore = Instantiate(sporePrefab, side);
            spore.transform.localPosition = Vector3.zero;

            Spore sporeInstance = spore.GetComponent<Spore>();

            sporeInstance.Init(Spore.SporeMode.Trap, 50f);
        }

        foreach (Transform top in upperSpawnPoints)
        {
            GameObject spore = Instantiate(sporePrefab, top);
            spore.transform.localPosition = Vector3.zero;

            Spore sporeInstance = spore.GetComponent<Spore>();

            sporeInstance.Init(Spore.SporeMode.Air, 75f);
        }
    }
}