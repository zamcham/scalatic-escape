using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MushroomEnemy : MonoBehaviour
{
    [SerializeField] GameObject sporePrefab;

    [Header("Spore Settings")]
    [SerializeField] float sporeCreateInterval = 1f;

    [SerializeField] float sporeFadeDistance;
    [SerializeField] float sporeFadeDuration;

    // Turn it into a coroutine for now
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
}