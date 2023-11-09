using UnityEngine;

public class Managers : MonoBehaviour
{
    [SerializeField] GameObject[] managerPrefabs;

    void Awake()
    {
        InstantiateManagers();
    }

    void InstantiateManagers()
    {
        foreach (var manager in managerPrefabs)
        {
            Instantiate(manager, transform);
        }
    }
}