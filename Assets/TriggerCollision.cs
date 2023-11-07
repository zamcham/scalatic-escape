using UnityEngine;

public class TriggerCollision : MonoBehaviour
{
    public bool isHit { get; private set; }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isHit = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isHit = false;
    }
}