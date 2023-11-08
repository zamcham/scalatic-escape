using UnityEngine;

public class AutoScroll : MonoBehaviour
{
    [SerializeField] float speed = 10f;

    void FixedUpdate()
    {
        transform.position += transform.right * speed * Time.fixedDeltaTime;
    }
}