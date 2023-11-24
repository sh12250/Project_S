using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    [SerializeField]
    private bool x, y, z;
    [SerializeField]
    private Transform target;
    // Update is called once per frame
    void Update()
    {
        if (!target) return;

        transform.position = new Vector3(
             (x ? target.position.x : transform.position.x),
             (y ? target.position.y : transform.position.y),
             (z ? target.position.z : transform.position.z));
    }
}
