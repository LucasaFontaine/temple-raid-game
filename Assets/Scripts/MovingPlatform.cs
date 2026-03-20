using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float waitTime = 1f;

    private Vector3 target;
    private float waitTimer = 0f;
    private bool waiting = false;

    void Start()
    {
        target = pointB.position;
    }

    void Update()
    {
        if (waiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
                waiting = false;
            return;
        }

        // Move toward target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Switch target when reached
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            target = target == pointA.position ? pointB.position : pointA.position;
            waiting = true;
            waitTimer = waitTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            other.transform.SetParent(transform);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            other.transform.SetParent(null);
    }
}