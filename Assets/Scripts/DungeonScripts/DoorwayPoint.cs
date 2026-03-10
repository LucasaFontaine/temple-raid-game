using UnityEngine;

public class DoorwayPoint : MonoBehaviour
{
    private bool _isConnected = false;
    public bool isConnected
    {
        get => _isConnected;
        set
        {
            if (value == true)
                Debug.Log("isConnected set to TRUE on " + gameObject.name + "\n" + System.Environment.StackTrace);
            _isConnected = value;
        }
    }

    void Awake()
    {
        _isConnected = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isConnected ? Color.green : Color.red;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawRay(transform.position, transform.forward * 1f);
    }
}