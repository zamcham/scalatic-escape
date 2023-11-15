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
    IEnumerator Start()
    {
        float timer = 0f;
        
        while (true)
        {
            if (timer >= sporeCreateInterval)
            {
                timer = 0f;

                GameObject sporeObject = Instantiate(sporePrefab, transform.position, Quaternion.identity, null);

                Debug.Log(sporeObject.name);

                Vector2 direction = Random.onUnitSphere;
                direction.y = Mathf.Abs(direction.y); // Make it a hemisphere

                Spore spore = new Spore(sporeObject, direction);
            }
            else
            {
                timer += Time.deltaTime;

                foreach (Spore spore in Spore.spores)
                {
                    if (!spore.isFading && Vector2.Distance(spore.transform.position, transform.position) >= sporeFadeDistance)
                    {
                        spore.Fade(this, sporeFadeDuration);
                    }

                    Vector3 direction = spore.direction;
                    spore.transform.position += direction * Time.deltaTime;
                }
            }

            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    class Spore
    {
        public int id;
        public static List<Spore> spores { get; private set; } = new List<Spore>();

        public GameObject gameObject;
        public Transform transform => gameObject.transform;

        public Vector2 direction;

        public bool isFading { get; private set; } = false;

        public Spore(GameObject gameObject, Vector2 direction)
        {
            this.gameObject = gameObject;

            id = spores.Count;
            spores.Add(this);

            Debug.Log(id);
            this.direction = direction;
        }

        public static void ResetSpores()
        {
            spores.Clear();
        }

        public void Fade(MonoBehaviour caller, float duration)
        {
            caller.StartCoroutine(Fade(duration));
        } 

        IEnumerator Fade(float duration)
        {      
            isFading = true;

            float lerp = 0f;

            SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
            Color startingColor = renderer.color;

            Color targetColor = renderer.color;
            targetColor.a = 0f;

            while (lerp <= 1f)
            {
                yield return new WaitForEndOfFrame();

                lerp += Time.deltaTime / duration;

                renderer.color = Color.Lerp(startingColor, targetColor, lerp);

                yield return null;
            }

            Destroy(gameObject);
            RemoveSpore(id);
        }

        public static void RemoveSpore(int id)
        {
            foreach (var spore in spores)
            {
                if (spore.id == id)
                {
                    spores.Remove(spore);
                    break;
                }
            }
        }
    }
}