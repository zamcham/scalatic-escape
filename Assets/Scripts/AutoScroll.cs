using UnityEngine;

public class AutoScroll : MonoBehaviour
{
    [SerializeField] float speed = 10f;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.fixedDeltaTime;
    }
}